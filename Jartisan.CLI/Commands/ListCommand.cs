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

                // --- CABEÇALHO ESTILIZADO ---
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n========================================");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   Jartisan | Project Dependencies 📦");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("========================================\n");
                Console.ResetColor();

                if (!dependencies.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  [!] No dependencies found in your pom.xml.");
                    Console.ResetColor();
                    return;
                }

                // --- LISTAGEM DE DEPENDÊNCIAS ---
                foreach (var depRaw in dependencies)
                {
                    // Se depRaw for nulo ou vazio, ignora para evitar erros
                    if (string.IsNullOrWhiteSpace(depRaw)) continue;

                    // Indicador de Sucesso/Presença em Verde Brilhante
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("  ✔  ");

                    // Quebra a string por ':' (Ex: "org.projectlombok:lombok:1.18.30")
                    var parts = depRaw.Split(':', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 2)
                    {
                        var groupId = parts[0].Trim();
                        var artifactId = parts[1].Trim();
                        
                        // Exibe Grupo e Artefato em Branco
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"{groupId}:{artifactId}");

                        // Se houver uma versão informada na terceira parte, pinta em cinza
                        if (parts.Length >= 3)
                        {
                            var version = parts[2].Trim();
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($" (v{version})");
                        }
                        else
                        {
                            Console.WriteLine(); // Apenas quebra a linha caso não tenha versão
                        }
                    }
                    else
                    {
                        // Caso a string não esteja no padrão esperado, imprime ela inteira em branco
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(depRaw);
                    }
                    
                    Console.ResetColor();
                }

                // --- RODAPÉ COM RESUMO QUANTITATIVO ---
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n----------------------------------------");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("  Total: ");
                
                Console.ForegroundColor = ConsoleColor.Green; // Destaca o número final em verde
                Console.Write(dependencies.Count);
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                if(dependencies.Count == 1)
                    Console.WriteLine(" dependency found.");
                else
                    Console.WriteLine(" dependencies found.");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
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
