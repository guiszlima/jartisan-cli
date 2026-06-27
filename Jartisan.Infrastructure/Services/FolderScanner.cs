using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Infrastructure.Services;

public class FolderScanner : IFolderScanner
{
    private readonly string _rootPath = Directory.GetCurrentDirectory();

    
    private static readonly Dictionary<string, string[]> _folderAliases = new()
    {
        ["Controllers"]  = ["Controllers", "Controller"],
        ["Models"]       = ["Models", "Model"],
        ["Services"]     = ["Services", "Service"],
        ["Repositories"] = ["Repositories", "Repository", "Repos", "Repo"],
        ["Dtos"]         = ["Dtos", "Dto"]
    };

    public FolderMap Scan()
    {
     string pomPath = Path.Combine(_rootPath, "pom.xml");
    bool isMavenProject = File.Exists(pomPath);
        var folderPaths = GetFolderPaths(_rootPath, _folderAliases);

        return new FolderMap(
            RootPath: _rootPath,
            Controllers: folderPaths.GetValueOrDefault("Controllers"),
            Models: folderPaths.GetValueOrDefault("Models"),
            Services: folderPaths.GetValueOrDefault("Services"),
            Repositories: folderPaths.GetValueOrDefault("Repositories"),
            Dtos: folderPaths.GetValueOrDefault("Dtos"),
            ScannedAt: DateTime.UtcNow
        );
    }

    public static Dictionary<string, string?> GetFolderPaths(string root, Dictionary<string, string[]> aliasMap)
    {
        if (!Directory.Exists(root)) return new Dictionary<string, string?>();

        var existingDirs = Directory.GetDirectories(root);

        return aliasMap.ToDictionary(
            entry => entry.Key,
            entry =>
            {
                var foundPath = existingDirs.FirstOrDefault(dir =>
                    
                    entry.Value.Any(alias =>
                        
                        Path.GetFileName(dir).Equals(alias, StringComparison.OrdinalIgnoreCase)));

                return foundPath != null ? Path.Combine(foundPath, string.Empty) : null;
            }
        );
    }
}