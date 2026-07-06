using Jartisan.Domain.Entities;    

namespace Jartisan.Application.Ports
{
    public interface IJsonManager
    {
        void SaveJsonConfig(FolderMap folderMap); 
        FolderMap LoadConfig();
    }
}