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
            services.AddTransient<UseCases.Init.InitTemplatesUseCase>();
            //Dependencias do comando Make
            services.AddTransient<UseCases.Make.MakeUseCase>();
            //Dependencias do comando Scan
            services.AddTransient<UseCases.Scan.ScanUseCase>();
           //Dependencias do comando Add
            services.AddTransient<UseCases.Add.AddUseCase>();
            return services;
        }
    }
}