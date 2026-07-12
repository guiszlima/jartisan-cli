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

        public MakeUseCase(IProjectDetector detector, ITemplateProcessor processor, IFileWriter writer, IJsonManager jsonManager)
        {
            _detector = detector;
            _processor = processor;
            _writer = writer;
            _jsonManager = jsonManager;
        }

        public void Execute(string scaffoldingType, string inputName, MakeOptions opts)
        {
            // 1. Basic fail-fast input validation
            if (string.IsNullOrWhiteSpace(inputName))
                throw new ArgumentException("The component name cannot be empty.", nameof(inputName));
            if (string.IsNullOrWhiteSpace(scaffoldingType))
                throw new ArgumentException("The scaffolding type cannot be empty.", nameof(scaffoldingType));

            // 2. Loads configurations from JSON cache (Deixe a exceção subir livremente aqui)
            FolderMap jsonData = _jsonManager.LoadConfig();
            
            // Protects against non-existent scaffolding types or typos in jartisan.json
            string targetPath = jsonData[scaffoldingType]
                ?? throw new InvalidOperationException($"[Error] The scaffolding type '{scaffoldingType}' is not mapped in your jartisan.json file.");

            // 3. Collects basic Java data
            string package = _processor.ProcessPackage(targetPath);
            string className = inputName.Trim();

            // 4. Builds the physical file name
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

            // If the infrastructure signals it needed to create the folder, the application outputs the print!
            if (wasFolderCreated)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Warning] The scaffolding folder did not exist physically. Directory created automatically.");
                Console.ResetColor();
            }
        }
    }
}
