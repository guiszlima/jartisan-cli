using System;
using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Application.UseCases.Scan
{
    public class ScanUseCase
    {
        private readonly IFolderScanner _folderScanner;
        private readonly IJsonManager _jsonManager;
        private readonly IProjectDetector _detector;

        public ScanUseCase(IFolderScanner folderScanner, IJsonManager jsonManager, IProjectDetector detector)
        {
            _folderScanner = folderScanner;
            _jsonManager = jsonManager;
            _detector = detector;
        }

        public FolderMap Execute()
        {
            
            if (!_detector.ProjectExists())
            {
                throw new InvalidOperationException("Não foi detectado um projeto Maven válido na pasta atual para escanear.");
            }


            FolderMap currentDiskMap = _folderScanner.Scan();


            _jsonManager.SaveJsonConfig(currentDiskMap);

            return currentDiskMap;
        }
    }
}
