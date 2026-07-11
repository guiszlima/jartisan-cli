using System.Text.Json;
using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;
using Jartisan.Infrastructure.Implementations.Serialization;

namespace Jartisan.Infrastructure.Implementations.Configuration;

public class JsonManager : IJsonManager

{
     private readonly string _rootPath = Directory.GetCurrentDirectory();
    public void SaveJsonConfig(FolderMap folderMap)
    {
        if (folderMap == null) 
            throw new ArgumentNullException(nameof(folderMap));

        string jsonString = JsonSerializer.Serialize(folderMap, JartisanJsonContext.Default.FolderMap);

        string filePath = Path.Combine(folderMap.RootPath, "jartisan.json");
        File.WriteAllText(filePath, jsonString);
    }

    public FolderMap LoadConfig()
    {
        string filePath = Path.Combine(_rootPath, "jartisan.json");
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The configuration file was not found at {filePath}");

        string jsonString = File.ReadAllText(filePath);
        
        var config = JsonSerializer.Deserialize<FolderMap>(jsonString, JartisanJsonContext.Default.FolderMap);
        
        return config ?? throw new InvalidOperationException("Failed to deserialize the jartisan.json file");
    }
}