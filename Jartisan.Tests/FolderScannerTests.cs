using System.IO;
using Jartisan.Infrastructure.Services;

namespace Jartisan.Tests;

public class FolderScannerTests
{
    [Fact]
    public void Scan_ReturnsFoundFolderPaths_WhenAliasMatchesExistingDirectories()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            Directory.CreateDirectory(Path.Combine(tempRoot, "Controller"));
            Directory.CreateDirectory(Path.Combine(tempRoot, "Models"));
            Directory.CreateDirectory(Path.Combine(tempRoot, "Service"));
            Directory.CreateDirectory(Path.Combine(tempRoot, "Repo"));
            Directory.CreateDirectory(Path.Combine(tempRoot, "Dtos"));

            string originalCurrent = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempRoot);

            try
            {
                var scanner = new FolderScanner();
                var result = scanner.Scan();

                Assert.Equal(tempRoot, result.RootPath);
                Assert.Equal(Path.Combine(tempRoot, "Controller", string.Empty), result.Controllers);
                Assert.Equal(Path.Combine(tempRoot, "Models", string.Empty), result.Models);
                Assert.Equal(Path.Combine(tempRoot, "Service", string.Empty), result.Services);
                Assert.Equal(Path.Combine(tempRoot, "Repo", string.Empty), result.Repositories);
                Assert.Equal(Path.Combine(tempRoot, "Dtos", string.Empty), result.Dtos);
                Assert.True(result.ScannedAt <= DateTime.UtcNow);
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
    public void Scan_ReturnsNullForMissingFolders()
    {
        string tempRoot = CreateTempFolder();
        try
        {
            Directory.CreateDirectory(Path.Combine(tempRoot, "Models"));

            string originalCurrent = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempRoot);

            try
            {
                var scanner = new FolderScanner();
                var result = scanner.Scan();

                Assert.Equal(tempRoot, result.RootPath);
                Assert.Null(result.Controllers);
                Assert.Equal(Path.Combine(tempRoot, "Models", string.Empty), result.Models);
                Assert.Null(result.Services);
                Assert.Null(result.Repositories);
                Assert.Null(result.Dtos);
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
