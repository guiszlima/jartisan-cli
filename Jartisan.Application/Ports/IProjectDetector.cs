
namespace Jartisan.Application.Ports
{
    public interface IProjectDetector
    {
      
        bool ProjectExists();
        string GetGroupId();
    }
}