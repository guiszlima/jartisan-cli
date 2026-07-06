using Jartisan.Infrastructure.Services;
using Jartisan.Application.Ports;
using Microsoft.Extensions.DependencyInjection;
namespace Jartisan.Infrastructure
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            services.AddTransient<IProjectDetector, JavaProjectDetector>();
            services.AddTransient<IProjectFactory, JavaProjectFactory>();
            services.AddTransient<IJsonManager, JsonManager>();
            services.AddTransient<IFolderScanner, FolderScanner>();

            services.AddTransient<ITemplateProcessor, TemplateProcessor>();
            services.AddTransient<IFileWriter, FileWriter>();
            
            return services;
        }

    }
}