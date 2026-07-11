

using System.Threading.Tasks; 
using Jartisan.Domain.Models;
namespace Jartisan.Application.Ports

{
    public interface IDependencyResolver
    {
        Task<List<DependencyInfo>> ResolveAsync(string query, CancellationToken cancellationToken);
    }
}