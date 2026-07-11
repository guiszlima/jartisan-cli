

using Jartisan.Application.Ports;
using System.Xml.Linq;

namespace Jartisan.Infrastructure.Implementations.Projects;

public class JavaProjectDetector : IProjectDetector
{
    private readonly string _rootPath;
    private readonly string _pomPath;

    public JavaProjectDetector()
    {
        _rootPath = Directory.GetCurrentDirectory();
        _pomPath = Path.Combine(_rootPath, "pom.xml");
    }

    public bool ProjectExists()
    {
        bool pomXmlExists = File.Exists(Path.Combine(_rootPath, "pom.xml"));
        bool javaStructureExists = Directory.Exists(Path.Combine(_rootPath, "src", "main", "java"));

        return pomXmlExists && javaStructureExists;
    }

    public string GetGroupId()
    {
        if (!File.Exists(_pomPath))
        {
            throw new FileNotFoundException($"O arquivo pom.xml não foi encontrado em: {_pomPath}");
        }

        XDocument doc = XDocument.Load(_pomPath);
        if (doc.Root == null)
            throw new InvalidOperationException("O arquivo pom.xml está vazio ou corrompido.");

        var groupId = doc.Root.Elements()
            .FirstOrDefault(e => e.Name.LocalName.Equals("groupId", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(groupId))
        {
            var parentElement = doc.Root.Elements()
                .FirstOrDefault(e => e.Name.LocalName.Equals("parent", StringComparison.OrdinalIgnoreCase));

            groupId = parentElement?.Elements()
                .FirstOrDefault(e => e.Name.LocalName.Equals("groupId", StringComparison.OrdinalIgnoreCase))?.Value;
        }

        return groupId ?? throw new InvalidOperationException("GroupId não encontrado no pom.xml");
    }

    public string GetArtifactId()
    {
        if (!File.Exists(_pomPath))
        {
            throw new FileNotFoundException($"O arquivo pom.xml não foi encontrado em: {_pomPath}");
        }

        XDocument doc = XDocument.Load(_pomPath);

        XNamespace nmspace = "http://maven.apache.org/POM/4.0.0";
        var artifactId = doc.Root?.Element(nmspace + "artifactId")?.Value;
        return artifactId ?? throw new InvalidOperationException("ArtifactId não encontrado no pom.xml");
    }
}