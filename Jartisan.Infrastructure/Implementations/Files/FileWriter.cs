using System.IO;
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Implementations.Files;

public class FileWriter : IFileWriter
{
    public bool Write(string path, string content)
    {
        bool folderCreated = false;
        string? directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            // If the folder doesn't physically exist, records that it will be created
            if (!Directory.Exists(directory))
            {
                folderCreated = true;
            }

            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, content);
        return folderCreated; // Returns pure technical information to the application
    }
}
