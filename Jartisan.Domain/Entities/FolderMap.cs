namespace Jartisan.Domain.Entities;

public record class FolderMap(
    
        string RootPath,
        string? Controllers, 
        string? Models,
        string? Services,     
        string? Repositories,  
        string? Dtos,          
        DateTime ScannedAt
    
    );