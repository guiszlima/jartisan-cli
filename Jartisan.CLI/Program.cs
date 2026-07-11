using Microsoft.Extensions.DependencyInjection;
using ConsoleAppFramework;
using Jartisan.Application;
using Jartisan.CLI.Commands;
using Jartisan.Infrastructure;


namespace Jartisan.CLI;

class Program
{
    static async Task Main(string[] args)
    {
        var app = ConsoleApp.Create();

        app.ConfigureServices(services =>
        {
            // Registre os UseCases aqui
            services.AddUseCases();
            services.AddInfrastructure();

    
        });
        
        app.Add<InitCommand>();
        app.Add<MakeCommand>();
        app.Add<ScanCommand>();
        app.Add<AddCommand>();

        
        await app.RunAsync(args);
    }
}