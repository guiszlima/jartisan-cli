using Jartisan.Domain.Entities;
using Jartisan.Infrastructure.Services;

namespace Jartisan.CLI;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Escaneando diretório atual...");
        Console.WriteLine(Directory.GetCurrentDirectory());

        var scanner = new FolderScanner();
        FolderMap folderMap = scanner.Scan();

        // Mostra no console o que foi encontrado, pra você ver se está certo
        Console.WriteLine($"RootPath: {folderMap.RootPath}");
        Console.WriteLine($"Controllers: {folderMap.Controllers ?? "(não encontrado)"}");
        Console.WriteLine($"Models: {folderMap.Models ?? "(não encontrado)"}");
        Console.WriteLine($"Services: {folderMap.Services ?? "(não encontrado)"}");
        Console.WriteLine($"Repositories: {folderMap.Repositories ?? "(não encontrado)"}");
        Console.WriteLine($"Dtos: {folderMap.Dtos ?? "(não encontrado)"}");

        var jsonManager = new JsonManager();
        jsonManager.SaveTemplate(folderMap);

        Console.WriteLine("Template salvo com sucesso em jartisan.json!");
    }
}