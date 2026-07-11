
using Jartisan.Domain.Models;

namespace Jartisan.Application.Ports
{
    public interface IDependencyEditor
    {
        bool AddDependency(DependencyInfo dependency);
    }
}