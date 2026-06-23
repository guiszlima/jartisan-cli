using Jartisan.Domain.Entities;

namespace Jartisan.Application.Ports;

public interface IFolderScanner
{
    FolderMap Scan();
}