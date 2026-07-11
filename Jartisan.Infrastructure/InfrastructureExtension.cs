using Jartisan.Application.Ports;
using Jartisan.Infrastructure.Implementations.Configuration;
using Jartisan.Infrastructure.Implementations.Files;
using Jartisan.Infrastructure.Implementations.Projects;
using Jartisan.Infrastructure.Implementations.Scanning;
using Jartisan.Infrastructure.Implementations.Templates;
using Jartisan.Infrastructure.Implementations.Maven;
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

            services.AddTransient<IDependencyResolver, MavenApi>();
            services.AddTransient<IDependencyEditor, PomEditor>();
            services.AddTransient<IDependencyReader, PomReader>();
            
            return services;
        }

    }
}