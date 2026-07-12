using System;
using System.Threading;
using System.Threading.Tasks;
using Jartisan.Application.Ports;
using Jartisan.Domain.Models;


namespace Jartisan.Application.UseCases.Add
{
    public class AddUseCase
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly IDependencyEditor _pomEditor;

        public AddUseCase(IDependencyResolver dependencyResolver, IDependencyEditor pomEditor)
        {
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            _pomEditor = pomEditor ?? throw new ArgumentNullException(nameof(pomEditor));
        }

        // Returns DependencyInfo if successfully added, or null if not found/failed
        public async Task<List<DependencyInfo>> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query)) return null;

            // 1. Busca os dados na API
            var dependency = await _dependencyResolver.ResolveAsync(query, cancellationToken);
            if (dependency == null) return null;

            
            

            return dependency;
        }
        public bool AddDependency(DependencyInfo dependency)
        {
            return _pomEditor.AddDependency(dependency);
        }
    }
}
