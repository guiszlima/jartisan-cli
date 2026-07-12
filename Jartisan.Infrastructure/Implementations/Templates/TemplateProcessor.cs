using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Jartisan.Application.Ports;
using Jartisan.Application.Utils; // Imports your Utils folder where the KebabRegex class is

namespace Jartisan.Infrastructure.Implementations.Templates;

public class TemplateProcessor : ITemplateProcessor
{
    private readonly IProjectDetector _projectDetector;

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

    public string Process(string templateName, Dictionary<string, string> replacements, bool isCrud = false)
    {
        string finalTemplateName = isCrud ? $"{templateName.ToLower()}_crud" : templateName.ToLower();
        string templateContent = LoadTemplateContent(finalTemplateName);

        if (replacements.TryGetValue("ClassName", out string? className) && !string.IsNullOrWhiteSpace(className))
        {
            replacements["endpointName"] = KebabRegex.Convert(className);
        }

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

        string customDiskPath = Path.Combine(Environment.CurrentDirectory, "templates.jartisan", fileName);
        if (File.Exists(customDiskPath)) return File.ReadAllText(customDiskPath);

        string embeddedPath = $"templates/{fileName}";
        string embeddedContent = LoadEmbeddedTemplate(embeddedPath);
        if (embeddedContent != null) return embeddedContent;

        throw new FileNotFoundException($"Template not found: {fileName}");
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
            throw new ArgumentException("The destination path cannot be null or empty.", nameof(targetPath));
        }

        string normalizedPath = Path.GetFullPath(targetPath);
        string sourceRoot = Path.Combine("src", "main", "java");

        int index = normalizedPath.IndexOf(sourceRoot, StringComparison.OrdinalIgnoreCase);
        if (index == -1)
        {
            throw new InvalidOperationException($"The target directory must be inside 'src/main/java'. Received path: {targetPath}");
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

            if (JavaKeywords.Contains(segment))
            {
                segment = "_" + segment;
            }

            segments[i] = segment.ToLower();
        }

        return string.Join(".", segments);
    }
}
