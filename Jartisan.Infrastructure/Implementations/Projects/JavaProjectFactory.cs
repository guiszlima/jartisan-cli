using System;
using System.Collections.Generic;
using System.IO;
using Jartisan.Domain.Entities;
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Implementations.Projects;
    // 1. Configuration object with default values (optional)
   
    public class JavaProjectFactory : IProjectFactory
    {
        private readonly IProjectDetector _projectDetector;
       
        public JavaProjectFactory(IProjectDetector projectDetector)
        {
            _projectDetector = projectDetector ?? throw new ArgumentNullException(nameof(projectDetector));
        }

        public void CreateProject(JavaProjectConfig? config = null)
        {
            config ??= new JavaProjectConfig();
            var projectName = new DirectoryInfo(_projectDetector.RootPath).Name;
            Console.WriteLine($"Creating Maven project with GroupId: {config.GroupId}, ArtifactId: {config.ArtifactId ?? projectName}, Version: {config.Version}, {_projectDetector.RootPath}");
            // If the user didn't type anything, assume the folder, otherwise keep what the user typed
            config.ArtifactId ??= projectName; 

            // CHANGE: Use config.ArtifactId here, not project folder name
            CreateJavaDirectoryStructure(config.GroupId);
            CreatePomXmlFile(config.GroupId, config.ArtifactId, config.Version);
        }


private void CreateJavaDirectoryStructure(string groupId)
    {
        string[] packageFolders = groupId.Split('.');
        var pathParts = new List<string> { _projectDetector.RootPath, "src", "main", "java" };
        pathParts.AddRange(packageFolders);

        string fullJavaPath = Path.Combine(pathParts.ToArray());
        Directory.CreateDirectory(fullJavaPath);

        string resourcesPath = Path.Combine(_projectDetector.RootPath, "src", "main", "resources");
        Directory.CreateDirectory(resourcesPath);
    }


       
 private void CreatePomXmlFile(string groupId, string artifactId, string version)
        {
            var pomPath = _projectDetector.PomPath;
            
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

