using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleAppFramework;
using Jartisan.Application.UseCases.Make;
using Jartisan.Domain.Entities;

namespace Jartisan.CLI.Commands
{
    public class MakeCommand(MakeUseCase makeUseCase)
    { 
       /// <summary>
    /// [Make] Generates code scaffolding for a specific artifact type (e.g., controller, service).
    /// </summary>
    /// <param name="scaffoldingType">The type of artifact to generate (e.g., controller, service, repository).</param>
    /// <param name="inputName">The name of the class or component to be created.</param>
    /// <param name="crud">-c, --crud, Automatically generates full CRUD operations for the artifact.</param>
    /// <param name="force">-f, --force, Overwrites existing files if they already exist.</param>
     [Command("make|mk")]
        
        public void Execute(
            [Argument] string scaffoldingType, // Parâmetro posicional obrigatório
            [Argument] string inputName, 
            bool crud = false, // Flag opcional '--crud' automática
            bool force = false // Flag opcional '--force' automática
        )
        {
            try
            {
                var options = new MakeOptions(IsCrud: crud , IsForce: force);
                
                makeUseCase.Execute(scaffoldingType, inputName, options);

                Console.ForegroundColor = ConsoleColor.Green;
                string artifactName = char.ToUpper(scaffoldingType[0]) + scaffoldingType.Substring(1).ToLower();
                Console.WriteLine($"\n[Success] {artifactName} generated successfully for '{inputName}'.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Error] {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
