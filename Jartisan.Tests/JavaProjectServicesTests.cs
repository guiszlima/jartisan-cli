using System.IO;
using Jartisan.Domain.Entities;
using Jartisan.Infrastructure.Services;

namespace Jartisan.Tests;

public class JavaProjectServicesTests :
{
    [Fact]
    public void ProjectExists_ReturnsTrue_WhenPomAndJavaStructureExist()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            File.WriteAllText(Path.Combine(tempRoot, "pom.xml"), "<project></project>");
            Directory.CreateDirectory(Path.Combine(tempRoot, "src", "main", "java"));

            string originalCurrent = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempRoot);
            try
            {
                var detector = new JavaProjectDetector();
                Assert.True(detector.ProjectExists());
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCurrent);
            }
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    [Fact]
    public void ProjectExists_ReturnsFalse_WhenPomXmlIsMissing()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            Directory.CreateDirectory(Path.Combine(tempRoot, "src", "main", "java"));

            string originalCurrent = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempRoot);
            try
            {
                var detector = new JavaProjectDetector();
                Assert.False(detector.ProjectExists());
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCurrent);
            }
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    [Fact]
    public void CreateProject_CreatesJavaDirectoryStructureAndPomFile_WithCustomConfiguration()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            string originalCurrent = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempRoot);
            try
            {
                var factory = new JavaProjectFactory();
                var config = new JavaProjectConfig
                {
                    GroupId = "com.example",
                    ArtifactId = "my-app",
                    Version = "2.0"
                };

                factory.CreateProject(config);

                Assert.True(Directory.Exists(Path.Combine(tempRoot, "src", "main", "java", "com", "example")));
                Assert.True(Directory.Exists(Path.Combine(tempRoot, "src", "main", "resources")));
                Assert.True(File.Exists(Path.Combine(tempRoot, "pom.xml")));

                string pomContent = File.ReadAllText(Path.Combine(tempRoot, "pom.xml"));
                Assert.Contains("<groupId>com.example</groupId>", pomContent);
                Assert.Contains("<artifactId>my-app</artifactId>", pomContent);
                Assert.Contains("<version>2.0</version>", pomContent);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCurrent);
            }
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    [Fact]
    public void CreateProject_UsesDirectoryNameAsArtifactId_WhenArtifactIdIsNull()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            string originalCurrent = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempRoot);
            try
            {
                var factory = new JavaProjectFactory();
                var config = new JavaProjectConfig
                {
                    GroupId = "org.example",
                    ArtifactId = null,
                    Version = "1.0"
                };

                factory.CreateProject(config);

                string pomContent = File.ReadAllText(Path.Combine(tempRoot, "pom.xml"));
                Assert.Contains($"<artifactId>{new DirectoryInfo(tempRoot).Name}</artifactId>", pomContent);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalCurrent);
            }
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    private static string CreateTempFolder()
    {
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        return folder;
    }
}
