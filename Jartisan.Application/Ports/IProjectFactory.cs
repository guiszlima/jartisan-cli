using Jartisan.Domain.Entities;
namespace Jartisan.Application.Ports
{
    public interface IProjectFactory
    {
        void CreateProject(JavaProjectConfig? config = null);
    }
}