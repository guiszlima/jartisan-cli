using System.IO;
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Services
{
    public class FileWriter : IFileWriter
    {
        public bool Write(string path, string content)
        {
            bool folderCreated = false;
            string? directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
            {
                // Se a pasta não existe fisicamente, registra que ela será criada
                if (!Directory.Exists(directory))
                {
                    folderCreated = true;
                }

                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, content);
            return folderCreated; // Retorna a informação técnica pura para a aplicação
        }
    }
}
