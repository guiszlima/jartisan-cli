using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;
namespace Jartisan.Application.UseCases.Init;
    public class InitJsonUseCase
    {
        
        private readonly IFolderScanner _folderScanner;
        private readonly IJsonManager _jsonManager;
       

        public InitJsonUseCase(IFolderScanner folderScanner, IJsonManager jsonManager)
        {
            _folderScanner = folderScanner;
            _jsonManager = jsonManager;
          
        }

        public FolderMap Execute()
        {

       

           FolderMap folderMap = _folderScanner.Scan();

              _jsonManager.SaveTemplate(folderMap);
    
                return folderMap;

        }   

    }
