using Microsoft.Extensions.DependencyInjection;

namespace Jartisan.Application
{
    public static class UseCasesExtension
    {
          public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
           //Dependencias do comando Init
            services.AddTransient<UseCases.Init.InitDetectUseCase>();
            services.AddTransient<UseCases.Init.InitProjectUseCase>();
            services.AddTransient<UseCases.Init.InitJsonUseCase>();
            //Dependencias do comando Make
            services.AddTransient<UseCases.Make.MakeUseCase>();
            return services;
        }
    }
}