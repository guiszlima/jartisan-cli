using Jartisan.Domain.Entities;
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Services;
    // 1. Objeto de configuração com valores padrão (opcionais)
   
    public class JavaProjectFactory : IProjectFactory
    {
        private readonly string _rootPath;
        private readonly DirectoryInfo _dirInfo;
       
      public JavaProjectFactory()
        {
            // Agora o contexto é de instância, e _rootPath já existe
            _rootPath = Directory.GetCurrentDirectory();
            _dirInfo = new DirectoryInfo(_rootPath);
        }
        public void CreateProject(JavaProjectConfig? config = null)
        {

        config ??= new JavaProjectConfig();
    Console.WriteLine($"Criando projeto Maven com GroupId: {config.GroupId}, ArtifactId: {config.ArtifactId ?? _dirInfo.Name}, Version: {config.Version}, {_rootPath}");
    // Se o usuário não digitou nada, assume a pasta, senão, mantém o que o usuário digitou
    config.ArtifactId ??= this._dirInfo.Name; 

    // MUDANÇA: Use config.ArtifactId aqui, e não this._dirInfo.Name
    CreateJavaDirectoryStructure(config.GroupId);
    CreatePomXmlFile(config.GroupId, config.ArtifactId, config.Version);
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
<project xmlns=""http://maven.apache.org/POM/4.0.0"" 
         xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
         xsi:schemaLocation=""http://maven.apache.org/POM/4.0.0 http://maven.apache.org/xsd/maven-4.0.0.xsd"">
    <modelVersion>4.0.0</modelVersion>

    <groupId>{groupId}</groupId>
    <artifactId>{artifactId}</artifactId>
    <version>{version}</version>

    <properties>
        <maven.compiler.release>17</maven.compiler.release>
        <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
    </properties>
</project>";
}

    }

