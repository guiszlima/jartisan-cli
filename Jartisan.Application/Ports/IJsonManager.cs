using Jartisan.Domain.Entities;    

namespace Jartisan.Application.Ports
{
    public interface IJsonManager
    {
        void SaveTemplate(FolderMap folderMap); 
    }
}