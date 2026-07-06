using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleAppFramework;
using Jartisan.Application.UseCases.Scan;
using Jartisan.Domain.Entities;

namespace Jartisan.CLI.Commands
{
    public class ScanCommand(ScanUseCase scanUseCase)
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScanCommand))]
        [Command("scan")]
        
        public void Execute()
        {
            try
            {
                Console.WriteLine("Sincronizando estrutura de pastas com o disco físico...");

                
                FolderMap updatedMap = scanUseCase.Execute();


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[Sucesso] Projeto re-escaneado e jartisan.json sincronizado com sucesso!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Erro] Falha ao executar o scan: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
