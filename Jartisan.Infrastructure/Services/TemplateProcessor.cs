using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Jartisan.Application.Ports;
using Jartisan.Application.Utils; // Importa a sua pasta Utills onde está a classe KebabRegex

namespace Jartisan.Infrastructure.Services
{
    // Removeu o 'partial' daqui pois a Regex foi para a classe Utils.KebabRegex
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly IProjectDetector _projectDetector;

        // RECUPERADO: HashSet necessário para o funcionamento do ProcessPackage (AOT Safe)
        private static readonly HashSet<string> JavaKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue",
            "default", "do", "double", "else", "enum", "extends", "false", "final", "finally", "float", "for",
            "goto", "if", "implements", "import", "instanceof", "int", "interface", "long", "native", "new", "null",
            "package", "private", "protected", "public", "return", "short", "static", "strictfp", "super", "switch",
            "synchronized", "this", "throw", "throws", "transient", "true", "try", "void", "volatile", "while"
        };

        public TemplateProcessor(IProjectDetector projectDetector)
        {
            _projectDetector = projectDetector ?? throw new ArgumentNullException(nameof(projectDetector));
        }

        // Método Process Genérico e de Baixa Manutenção
        public string Process(string templateName, Dictionary<string, string> replacements, bool isCrud = false)
        {
          
            string finalTemplateName = isCrud ? $"{templateName.ToLower()}_crud" : templateName.ToLower();
            string templateContent = LoadTemplateContent(finalTemplateName);

          
            if (replacements.TryGetValue("ClassName", out string? className) && !string.IsNullOrWhiteSpace(className))
            {
                replacements["endpointName"] = KebabRegex.Convert(className);
            }

            // 3. Substituição de todas as chaves
            foreach (var replacement in replacements)
            {
                templateContent = templateContent.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
            }

            return templateContent;
        }


        private string LoadTemplateContent(string templateName)
        {
            string fileName = templateName.EndsWith(".tpl", StringComparison.OrdinalIgnoreCase) 
                ? Path.GetFileName(templateName) 
                : $"{templateName}.tpl";

            string embeddedPath = $"templates/{fileName}";
            string embeddedContent = LoadEmbeddedTemplate(embeddedPath);
            if (embeddedContent != null) return embeddedContent;

            string customDiskPath = Path.Combine(Environment.CurrentDirectory, "templates.jartisan", fileName);
            if (File.Exists(customDiskPath)) return File.ReadAllText(customDiskPath);

            throw new FileNotFoundException($"Template não encontrado: {fileName}");
        }

        private string LoadEmbeddedTemplate(string templatePath)
        {
            var assembly = typeof(TemplateProcessor).Assembly;
            string cleanName = templatePath.Replace("/", ".").Replace("\\", ".");
            string resourceName = $"Jartisan.Infrastructure.{cleanName}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) return null;

            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public string ProcessPackage(string targetPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                throw new ArgumentException("O caminho de destino não pode ser nulo ou vazio.", nameof(targetPath));
            }

            string normalizedPath = Path.GetFullPath(targetPath);
            string sourceRoot = Path.Combine("src", "main", "java");

            int index = normalizedPath.IndexOf(sourceRoot, StringComparison.OrdinalIgnoreCase);
            if (index == -1)
            {
                throw new InvalidOperationException($"O diretório alvo deve estar dentro de 'src/main/java'. Caminho recebido: {targetPath}");
            }

            string packagePart = normalizedPath.Substring(index + sourceRoot.Length);
            string[] segments = packagePart.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
            {
                return _projectDetector.GetGroupId();
            }

            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                segment = Regex.Replace(segment, @"[^a-zA-Z0-9_]", "_");

                if (segment.Length > 0 && char.IsDigit(segment[0]))
                {
                    segment = "_" + segment;
                }

                // O hashset foi restaurado no topo, então essa linha agora funciona perfeitamente!
                if (JavaKeywords.Contains(segment))
                {
                    segment = "_" + segment;
                }

                segments[i] = segment.ToLower(); 
            }

            return string.Join(".", segments);
        }
    }
}
