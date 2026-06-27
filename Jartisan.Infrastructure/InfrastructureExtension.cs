using Jartisan.Infrastructure.Services;
using Jartisan.Application.Ports;
using Microsoft.Extensions.DependencyInjection;
namespace Jartisan.Infrastructure
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Registre os serviços de infraestrutura aqui
            services.AddTransient<IProjectDetector, JavaProjectDetector>();
            services.AddTransient<IProjectFactory, JavaProjectFactory>();
            services.AddTransient<IJsonManager, JsonManager>();
            services.AddTransient<IFolderScanner, FolderScanner>();

            
            return services;
        }

    }
}