using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleAppFramework;
using Jartisan.Application.UseCases.Make;
using Jartisan.Domain.Entities;

namespace Jartisan.CLI.Commands
{
    public class MakeCommand(MakeUseCase makeUseCase)
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MakeCommand))]
        [Command("make")] // CORREÇÃO: Passa apenas o nome do comando aqui
        
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
                Console.WriteLine($"\n[Sucesso] {artifactName} gerado com sucesso para '{inputName}'.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Erro] {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
