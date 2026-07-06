

namespace Jartisan.Domain.Entities;

public record FolderMap(
    string RootPath,
    string? TemplatesFolder,
    string? Controllers, 
    string? Models,
    string? Services,     
    string? Repositories,  
    string? Dtos,
    IReadOnlyDictionary<string, string>? Custom = null
)
{
    // O INDEXER: Cria a sintaxe de colchetes tipo Python/JS para o seu objeto
    public string? this[string folderName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(folderName)) return null;

            // Normaliza o input do usuário para minúsculo
            string search = folderName.Trim().ToLower();

            // 1. Tenta buscar primeiro nas propriedades nativas
            string? nativePath = search switch
            {
                "controller" => Controllers,
                "model"      => Models,
                "service"    => Services,
                "repository" => Repositories,
                "dto"        => Dtos,
                _            => null
            };

            // 2. Se não for nativa, cai direto na busca do dicionário Custom
            if (nativePath == null && Custom != null)
            {
                Custom.TryGetValue(search, out nativePath);
            }

            return nativePath;
        }
    }
}
