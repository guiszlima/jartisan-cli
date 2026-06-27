using Jartisan.Domain.Entities;
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Services;
    // 1. Objeto de configuração com valores padrão (opcionais)
   
    public class JavaProjectFactory : IProjectFactory
    {
        private readonly string _rootPath = Directory.GetCurrentDirectory();

        // 2. Método principal com o parâmetro opcional configurado como 'null' por padrão
        public void CreateProject(JavaProjectConfig? config = null)
        {
           
            config ??= new JavaProjectConfig();

          
            string actualArtifactId = config.ArtifactId ?? new DirectoryInfo(_rootPath).Name;

            CreateJavaDirectoryStructure(config.GroupId);
            CreatePomXmlFile(config.GroupId, actualArtifactId, config.Version);
        }

          private void CreateJavaDirectoryStructure(string groupId)
        {
            string[] packageFolders = groupId.Split('.');
            var pathParts = new List<string> { _rootPath, "src", "main", "java" };
            pathParts.AddRange(packageFolders);

            string fullJavaPath = Path.Combine(pathParts.ToArray());
            Directory.CreateDirectory(fullJavaPath);

            string resourcesPath = Path.Combine(_rootPath, "src", "main", "resources");
            Directory.CreateDirectory(resourcesPath);
        }


        private void CreatePomXmlFile(string groupId, string artifactId, string version)
        {
            string pomPath = Path.Combine(_rootPath, "pom.xml");
            
            if (!File.Exists(pomPath))
            {
                string pomContent = GetPomContent(groupId, artifactId, version);
                File.WriteAllText(pomPath, pomContent);
            }
        }

        private string GetPomContent(string groupId, string artifactId, string version)
        {
            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<project xmlns=""http://apache.org""
         xmlns:xsi=""http://w3.org""
         xsi:schemaLocation=""http://apache.org http://apache.org"">
    <modelVersion>4.0.0</modelVersion>

    <groupId>{groupId}</groupId>
    <artifactId>{artifactId}</artifactId>
    <version>{version}</version>

    <properties>
        <maven.compiler.source>17</maven.compiler.source>
        <maven.compiler.target>17</maven.compiler.target>
    </properties>
</project>";
        }
    }

