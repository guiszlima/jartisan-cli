using Jartisan.Application.Ports;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Jartisan.Infrastructure.Implementations.Projects;

public class JavaProjectDetector : IProjectDetector
{
    public string RootPath { get; private set; } = string.Empty;
    public string PomPath { get; private set; } = string.Empty;
    private bool _isValidProject;

    public JavaProjectDetector()
    {
        // Heavy I/O moved to a private method called in the constructor
        // For better performance, consider extracting this to an .Initialize() method if the interface allows
        InitializePaths();
    }

    private void InitializePaths()
    {
        var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

        // 1. Search for jartisan.json + pom.xml
        while (currentDir != null)
        {
            var jsonCheck = Path.Combine(currentDir.FullName, "jartisan.json");
            var pomCheck = Path.Combine(currentDir.FullName, "pom.xml");

            if (File.Exists(jsonCheck) && File.Exists(pomCheck))
            {
                try
                {
                    string jsonString = File.ReadAllText(jsonCheck);
                    using var doc = JsonDocument.Parse(jsonString);
                    if (doc.RootElement.TryGetProperty("rootPath", out var rootProp))
                    {
                        RootPath = rootProp.GetString() ?? currentDir.FullName;
                        PomPath = pomCheck;
                        _isValidProject = true;
                        return;
                    }
                }
                catch
                {
                    // Ignores corrupted JSON, continues to the next level
                }
            }
            currentDir = currentDir.Parent;
        }

        // 2. FALLBACK: Only pom.xml (init scenario)
        currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (currentDir != null)
        {
            var pomCheck = Path.Combine(currentDir.FullName, "pom.xml");
            if (File.Exists(pomCheck))
            {
                RootPath = currentDir.FullName;
                PomPath = pomCheck;
                _isValidProject = true;
                return;
            }
            currentDir = currentDir.Parent;
        }

        // 3. Fora de um ecossistema Java
        RootPath = Directory.GetCurrentDirectory();
        PomPath = Path.Combine(RootPath, "pom.xml");
        _isValidProject = false;
    }

    public bool ProjectExists()
    {
        if (!_isValidProject) return false;
        return File.Exists(PomPath) && Directory.Exists(Path.Combine(RootPath, "src", "main", "java"));
    }

    public string GetGroupId()
    {
        var doc = LoadPomDocument();
        XNamespace ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

        // Busca o groupId direto do projeto
        var groupId = doc.Root?.Element(ns + "groupId")?.Value;

        // If not found, search in <parent>
        if (string.IsNullOrEmpty(groupId))
        {
            groupId = doc.Root?.Element(ns + "parent")?.Element(ns + "groupId")?.Value;
        }

        return groupId ?? throw new InvalidOperationException("GroupId not found in pom.xml");
    }

    public string GetArtifactId()
    {
        var doc = LoadPomDocument();
        XNamespace ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

        var artifactId = doc.Root?.Element(ns + "artifactId")?.Value;
        return artifactId ?? throw new InvalidOperationException("ArtifactId not found in pom.xml");
    }

    private XDocument LoadPomDocument()
    {
        if (!ProjectExists()) 
            throw new FileNotFoundException($"The pom.xml file was not found or the src/main/java structure is invalid. Path: {PomPath}");
        
        return XDocument.Load(PomPath);
    }
}
