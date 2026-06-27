using System.IO;
using System.Text.Json;
using Jartisan.Domain.Entities;
using Jartisan.Infrastructure.Services;

namespace Jartisan.Tests;

public class JsonManagerTests
{
    [Fact]
    public void SaveTemplate_WritesJartisanJsonFile_WhenFolderMapIsValid()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            var folderMap = new FolderMap(
                RootPath: tempRoot,
                Controllers: null,
                Models: null,
                Services: null,
                Repositories: null,
                Dtos: null,
                ScannedAt: DateTime.UtcNow);

            var manager = new JsonManager();
            manager.SaveTemplate(folderMap);

            string outputPath = Path.Combine(tempRoot, "jartisan.json");
            Assert.True(File.Exists(outputPath));

            string json = File.ReadAllText(outputPath);
            using var document = JsonDocument.Parse(json);
            Assert.Equal(tempRoot, document.RootElement.GetProperty("rootPath").GetString());
            DateTime parsedScannedAt = DateTime.Parse(document.RootElement.GetProperty("scannedAt").GetString()!).ToUniversalTime();
            Assert.Equal(folderMap.ScannedAt, parsedScannedAt);
        }
        finally
        {
            Directory.Delete(tempRoot, true);
        }
    }

    [Fact]
    public void SaveTemplate_ThrowsArgumentNullException_WhenFolderMapIsNull()
    {
        var manager = new JsonManager();
        Assert.Throws<ArgumentNullException>(() => manager.SaveTemplate(null!));
    }

    private static string CreateTempFolder()
    {
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        return folder;
    }
}
