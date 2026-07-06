using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Infrastructure.Services;

public class FolderScanner : IFolderScanner
{
    private readonly string _rootPath = Directory.GetCurrentDirectory();
      private readonly IProjectDetector _projectDetector;
    private static readonly Dictionary<string, string[]> _folderAliases = new()
    {
        // 1. ADICIONADO: Mapeamento para a pasta de Templates no root
        ["Templates"]    = ["templates.jartisan"], 
        ["Controllers"]  = ["Controllers", "Controller"],
        ["Models"]       = ["Models", "Model"],
        ["Services"]     = ["Services", "Service"],
        ["Repositories"] = ["Repositories", "Repository", "Repos", "Repo"],
        ["Dtos"]         = ["Dtos", "Dto"], 
    };

    
    public FolderScanner(IProjectDetector projectDetector)
    {
        _projectDetector = projectDetector;
    }


    public FolderMap Scan()
    {
        string pomPath = Path.Combine(_rootPath, "pom.xml");
        bool isMavenProject = File.Exists(pomPath);
        
        var folderPaths = GetFolderPaths(_rootPath, _folderAliases);


      string  baseJavaPath = this.getJavaPath();
        // 4. Aplica o fallback paliativo matemático usando o operador ??
        return new FolderMap(
            RootPath: _rootPath,
            TemplatesFolder: folderPaths.GetValueOrDefault("Templates")    ?? Path.Combine(_rootPath, "templates.jartisan"),
            Controllers:     folderPaths.GetValueOrDefault("Controllers")  ?? Path.Combine(baseJavaPath, "Controller"),
            Models:          folderPaths.GetValueOrDefault("Models")       ?? Path.Combine(baseJavaPath, "Model"),
            Services:        folderPaths.GetValueOrDefault("Services")     ?? Path.Combine(baseJavaPath, "Service"),
            Repositories:    folderPaths.GetValueOrDefault("Repositories") ?? Path.Combine(baseJavaPath, "Repository"),
            Dtos:            folderPaths.GetValueOrDefault("Dtos")         ?? Path.Combine(baseJavaPath, "Dto")
        );
    }

    public static Dictionary<string, string?> GetFolderPaths(string root, Dictionary<string, string[]> aliasMap)
    {
        if (!Directory.Exists(root)) return new Dictionary<string, string?>();

        var allDirs = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);

        var existingDirs = allDirs.Where(dir => 
                !dir.Contains($"{Path.DirectorySeparatorChar}target{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                !dir.Contains($"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) &&
                !dir.EndsWith($"{Path.DirectorySeparatorChar}target", StringComparison.OrdinalIgnoreCase) &&
                !dir.EndsWith($"{Path.DirectorySeparatorChar}.git", StringComparison.OrdinalIgnoreCase)
            ).ToArray();

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

    public string getJavaPath()
    {
                 string groupId = _projectDetector.GetGroupId(); // ex: "com.jartisan"
        
        // Transforma os pontos do pacote em barras do sistema operacional
        string packageFolder = groupId.Trim().Replace(".", "/");
        string baseJavaPath = Path.Combine(_rootPath, "src", "main", "java", packageFolder);
        return baseJavaPath;
    }
}
