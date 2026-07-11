using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Jartisan.Application.UseCases.Add;
using Jartisan.Domain.Models;

namespace Jartisan.CLI.Commands
{
    public class AddCommand
    {
        private readonly AddUseCase _addUseCase;

        public AddCommand(AddUseCase addUseCase)
        {
            _addUseCase = addUseCase ?? throw new ArgumentNullException(nameof(addUseCase));
        }
 
      /// <summary>
        /// Adds a dependency to the project's pom.xml file.
        /// </summary>
        /// <param name="query">The search query for finding the dependency.</param>
        [Command("add")] 
       
        public async Task HandleAsync([Argument] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                DisplayError("The search query cannot be empty.");
                return;
            }

            // 1. Fetch the results (asynchronously with CancellationToken)
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            Console.Write("Processing search on Maven Central... ");
            
            var searchTask = _addUseCase.SearchAsync(query, cts.Token);
            var results = await ShowLoadingAnimationAsync(searchTask);

            // 2. Validate the results
            if (results == null || results.Count == 0)
            {
                DisplayError("No dependencies were found for that query.");
                return;
            }

            // 3. Display the interactive list
            Console.WriteLine($"\nFound {results.Count} results:\n");
            for (int i = 0; i < results.Count; i++)
            {
                var dep = results[i];
                Console.WriteLine($" [{i + 1}] {dep.GroupId}:{dep.ArtifactId} (v{dep.Version})");
            }

            // 4. Seleção do usuário
            int selectedIndex = -1;
            while (true)
            {
                Console.Write("\nChoose the dependency number (or 0 to cancel): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int choice))
                {
                    if (choice == 0) 
                    {
                        Console.WriteLine("Operation cancelled by the user.");
                        return;
                    }
                    if (choice > 0 && choice <= results.Count)
                    {
                        selectedIndex = choice - 1;
                        break;
                    }
                }
                Console.WriteLine("Invalid option, please try again.");
            }

            // 5. Execução da adição
            var selected = results[selectedIndex];
            try
            {
                bool isAdded = _addUseCase.AddDependency(selected);
    
    Console.ForegroundColor = isAdded ? ConsoleColor.Green : ConsoleColor.Yellow;
    
    Console.WriteLine(isAdded 
        ? $"\n[Success] Added: {selected.GroupId}:{selected.ArtifactId} (v{selected.Version}) ✨" 
        : $"\n[Warning] The dependency {selected.GroupId}:{selected.ArtifactId} already exists in pom.xml.");
        
    Console.ResetColor();
            }
            catch (Exception ex)
            {
                DisplayError($"Error updating pom.xml: {ex.Message}");
            }
        }

        private static async Task<T?> ShowLoadingAnimationAsync<T>(Task<T?> task)
        {
            char[] spinnerChars = { '|', '/', '-', '\\' };
            int counter = 0;
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            try { Console.CursorVisible = false; } catch { }

            while (!task.IsCompleted)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(spinnerChars[counter % spinnerChars.Length]);
                counter++;
                await Task.Delay(100);
            }

            Console.SetCursorPosition(left, top);
            Console.Write(" "); // Limpa o spinner
            
            try { Console.CursorVisible = true; } catch { }

            return await task;
        }

        private static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[Error] {message}");
            Console.ResetColor();
        }
    }
}