// Arquivo: CLI/Commands/ListCommand.cs
using System;
using System.Linq;
using ConsoleAppFramework;
using Jartisan.Application.UseCases.List;

namespace Jartisan.CLI.Commands
{
    public class ListCommand
    {
        private readonly ListUseCase _useCase;

        public ListCommand(ListUseCase useCase)
        {
            _useCase = useCase;
        }

        // CORRECT ORDER: XML block first, followed by attribute attached to method signature
        /// <summary>
        /// Lists all dependencies currently defined in the pom.xml file.
        /// </summary>
        [Command("list")]
        public void Execute()
        {
            try
            {
                var dependencies = _useCase.Execute();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n========================================");
                Console.WriteLine("   Jartisan | Project Dependencies");
                Console.WriteLine("========================================\n");
                Console.ResetColor();

                if (!dependencies.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  [!] No dependencies found in your pom.xml.");
                    Console.ResetColor();
                    return;
                }

                foreach (var dep in dependencies)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("  -> ");
                    Console.ResetColor();
                    Console.WriteLine($"{dep}");
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n----------------------------------------");
                Console.WriteLine($"  Total: {dependencies.Count} dependency(ies) found.");
                Console.WriteLine("----------------------------------------\n");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Error] Failed to list dependencies: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
