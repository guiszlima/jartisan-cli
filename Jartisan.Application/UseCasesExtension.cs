using Microsoft.Extensions.DependencyInjection;

namespace Jartisan.Application
{
    public static class UseCasesExtension
    {
          public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
           
            services.AddTransient<UseCases.Init.InitDetectUseCase>();
            services.AddTransient<UseCases.Init.InitProjectUseCase>();
            services.AddTransient<UseCases.Init.InitJsonUseCase>();
            
            return services;
        }
    }
}