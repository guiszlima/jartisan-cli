using System.Text.Json;
using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Infrastructure.Services;

public class JsonManager : IJsonManager
{
    public void SaveTemplate(FolderMap folderMap)
    {
        if (folderMap == null) 
            throw new ArgumentNullException(nameof(folderMap));

        string jsonString = JsonSerializer.Serialize(folderMap, JartisanJsonContext.Default.FolderMap);

        string filePath = Path.Combine(folderMap.RootPath, "jartisan.json");
        File.WriteAllText(filePath, jsonString);
    }
}