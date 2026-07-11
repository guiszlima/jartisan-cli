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
                throw new ArgumentException("The component name cannot be empty.", nameof(inputName));
            if (string.IsNullOrWhiteSpace(scaffoldingType))
                throw new ArgumentException("The scaffolding type cannot be empty.", nameof(scaffoldingType));

            // 2. Carrega as configurações do cache JSON
            FolderMap jsonData = _jsonManager.LoadConfig();
            
            // Protege contra tipos de scaffolding inexistentes ou erros de digitação no jartisan.json
            string targetPath = jsonData[scaffoldingType]
                ?? throw new InvalidOperationException($"[Error] The scaffolding type '{scaffoldingType}' is not mapped in your jartisan.json file.");

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
                    $"The file '{fileName}' already exists in this directory. " +
                    "If you want to overwrite the current file, run the command again with the '--force' flag.");
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
                Console.WriteLine($"[Warning] The scaffolding folder did not exist physically. Directory created automatically.");
                Console.ResetColor();
            }
        }
    }
}
