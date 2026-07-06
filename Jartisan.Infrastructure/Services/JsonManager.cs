using System.Text.Json;
using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Infrastructure.Services;

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
            throw new FileNotFoundException($"O arquivo de configuração não foi encontrado em {filePath}");

        string jsonString = File.ReadAllText(filePath);
        
        var config = JsonSerializer.Deserialize<FolderMap>(jsonString, JartisanJsonContext.Default.FolderMap);
        
        return config ?? throw new InvalidOperationException("Falha ao desserializar o arquivo jartisan.json");
    }
}