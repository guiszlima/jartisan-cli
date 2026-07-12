using System;
using System.IO;
using System.Text.Json;
using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;
using Jartisan.Infrastructure.Implementations.Serialization;

namespace Jartisan.Infrastructure.Implementations.Configuration;

public class JsonManager : IJsonManager

{
    private readonly IProjectDetector _projectDetector;

    public JsonManager(IProjectDetector projectDetector)
    {
        _projectDetector = projectDetector ?? throw new ArgumentNullException(nameof(projectDetector));
    }

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
    string filePath = Path.Combine(_projectDetector.RootPath, "jartisan.json");
    
    // Checks if jartisan.json exists, throwing a clean operation exception for bad DevEx directory contexts
    if (!File.Exists(filePath))
    {
        throw new InvalidOperationException(
            "You are not inside an initialized Jartisan project or you are in the wrong directory. " +
            "Make sure to execute this command within a project directory that contains a 'jartisan.json' file.");
    }

    string jsonString = File.ReadAllText(filePath);
    
    var config = JsonSerializer.Deserialize<FolderMap>(jsonString, JartisanJsonContext.Default.FolderMap);
    
    return config ?? throw new InvalidOperationException("Failed to deserialize the jartisan.json file.");
}
}