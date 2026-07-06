using System;
using System.Collections.Generic;
using System.IO;
using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Application.UseCases.Make
{
    public class MakeUseCase
    {
        private readonly IProjectDetector _detector;
        private readonly ITemplateProcessor _processor;
        private readonly IFileWriter _writer;
        private readonly IJsonManager _jsonManager;


        public MakeUseCase(IProjectDetector detector, ITemplateProcessor processor, IFileWriter writer,
            IJsonManager jsonManager, IFolderScanner scanner)
        {
            _detector = detector;
            _processor = processor;
            _writer = writer;
            _jsonManager = jsonManager;
        }

        public void Execute(string scaffoldingType, string inputName, MakeOptions opts)
        {
            // 1. Fail-Fast básico de entrada
            if (string.IsNullOrWhiteSpace(inputName))
                throw new ArgumentException("O nome do componente não pode ser vazio.", nameof(inputName));
            if (string.IsNullOrWhiteSpace(scaffoldingType))
                throw new ArgumentException("O tipo de scaffolding não pode ser vazio.", nameof(scaffoldingType));

            // 2. Carrega as configurações do cache JSON
            FolderMap jsonData = _jsonManager.LoadConfig();
            
            // Protege contra tipos de scaffolding inexistentes ou erros de digitação no jartisan.json
            string targetPath = jsonData[scaffoldingType]
                ?? throw new InvalidOperationException($"[Erro] O tipo de scaffolding '{scaffoldingType}' não está mapeado no seu arquivo jartisan.json.");

            // 3. Coleta os dados básicos do Java
            string package = _processor.ProcessPackage(targetPath);
            string className = inputName.Trim();

            // 4. Monta o nome do arquivo físico (Sua lógica cirúrgica movida para cima para podermos testar o arquivo)
            string pureType = Path.GetFileNameWithoutExtension(scaffoldingType).Trim();
            string suffix = char.ToUpper(pureType[0]) + pureType.Substring(1).ToLower();
            string fileName = $"{className}{suffix}.java";
            string fullPath = Path.Combine(targetPath, fileName);


            if (File.Exists(fullPath) && !opts.IsForce)
            {
                throw new InvalidOperationException(
                    $"O arquivo '{fileName}' já existe neste diretório. " +
                    "Se deseja sobrescrever o arquivo atual, execute o comando novamente adicionando a flag '--force'.");
            }
     

            var replacements = new Dictionary<string, string>
            {
                { "package", package },
                { "ClassName", className }
            };

           
            string content = _processor.Process(scaffoldingType, replacements, opts.IsCrud);
            

            bool wasFolderCreated = _writer.Write(fullPath, content);

// Se a infraestrutura avisar que precisou criar a pasta, a aplicação solta o print!
            if (wasFolderCreated)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Aviso] A pasta do scaffolding não existia fisicamente. Diretório criado automaticamente.");
                Console.ResetColor();
            }
        }
    }
}
