namespace Jartisan.Domain.Entities;

public record  FolderMap(
    
        string RootPath,
        string? Controllers, 
        string? Models,
        string? Services,     
        string? Repositories,  
        string? Dtos,          
        DateTime ScannedAt
    
    );