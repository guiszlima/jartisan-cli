using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleAppFramework;
using Jartisan.Application.UseCases.Scan;
using Jartisan.Domain.Entities;

namespace Jartisan.CLI.Commands
{
    public class ScanCommand(ScanUseCase scanUseCase)
    {

         /// <summary>
        /// Scans the project directory structure and synchronizes it with the jartisan.json configuration.
        /// </summary>
        [Command("scan")]
        
        public void Execute()
        {
            try
            {
                Console.WriteLine("Synchronizing folder structure with the physical disk...");

                
                FolderMap updatedMap = scanUseCase.Execute();


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n[Success] Project re-scanned and jartisan.json synchronized successfully!");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Error] Failed to execute the scan: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
