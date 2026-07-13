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
/// Searches for and adds a Maven dependency to the project's pom.xml.
/// </summary>
/// <param name="query">
/// The search term or Maven coordinates to resolve.
/// 
/// Behaviors:
///   - Single match: If the search returns exactly one result, it is added automatically.
///   - Multiple matches: If multiple results are found, an interactive list is displayed.
/// 
/// Supported query formats:
///   - Search term : 'lombok'
///   - Exact       : 'groupId:artifactId:version'
///   - Partial     : 'groupId:artifactId' OR 'artifactId:version'
/// </param>
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

            // Control variable to store the selected dependency
            DependencyInfo selected;

            // NEW DEVEX LOGIC: If there is only 1 result, select it automatically
            if (results.Count == 1)
            {
                selected = results[0];
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n[Info] Single match found! Selecting automatically...");
                Console.ResetColor();
            }
 else
            {
                // Local Pagination Settings
                int selectedIndex = ShowPaginatedMenu(results);
                
                // Se o retorno for -1, significa que o usuário escolheu cancelar (0)
                if (selectedIndex == -1)
                {
                    Console.WriteLine("Operation cancelled by the user.");
                    return;
                }

                selected = results[selectedIndex];
            }

            // 5. Execution of addition
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

  
 private static int ShowPaginatedMenu(List<DependencyInfo> results)
{
    int pageSize = 10;
    int currentPage = 1;
    int totalItems = results.Count;
    int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

    int linesWritten = 0;

    while (true)
    {
        // Reseta o cursor e limpa o bloco anterior cirurgicamente via ANSI
        if (linesWritten > 0)
        {
            Console.Write($"\u001b[{linesWritten}A\u001b[J");
        }

        int currentLinesCount = 0;

        // --- TÍTULO ---
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n--- Search Results (Page {currentPage} of {totalPages}) ---");
        Console.ResetColor();
        currentLinesCount += 2;

        int startIndex = (currentPage - 1) * pageSize;
        int endIndex = Math.Min(startIndex + pageSize, totalItems);

        // --- LISTAGEM DE DEPENDÊNCIAS ---
        for (int i = startIndex; i < endIndex; i++)
        {
            var dep = results[i];
            
            // Cor do Número da Opção [X]
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($" [{i + 1}] ");
            
            // Cor do GroupId e ArtifactId
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{dep.GroupId}:{dep.ArtifactId}");
            
            // Cor da Versão
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" (v{dep.Version})");
            
            Console.ResetColor();
            currentLinesCount++;
        }

        // --- OPÇÕES DE NAVEGAÇÃO ---
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("\nNavigation Options:");
        Console.ResetColor();
        currentLinesCount += 2;
        
        if (currentPage < totalPages) 
        { 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [N] Next Page"); 
            Console.ResetColor();
            currentLinesCount++; 
        }
        if (currentPage > 1)          
        { 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [P] Previous Page"); 
            Console.ResetColor();
            currentLinesCount++; 
        }
        
        // Opção Cancelar em Vermelho
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("  [0] Cancel Process");
        Console.ResetColor();
        currentLinesCount++;

        // Prompt de Input
        Console.Write("\nChoose a dependency number or navigation command: ");
        currentLinesCount += 2; 

        linesWritten = currentLinesCount;

        var input = Console.ReadLine()?.Trim().ToUpper();

        if (input == "N" && currentPage < totalPages)
        {
            currentPage++;
            continue;
        }
        if (input == "P" && currentPage > 1)
        {
            currentPage--;
            continue;
        }

        if (int.TryParse(input, out int choice))
        {
            if (choice == 0) 
            {
                return -1; // Cancelado
            }
            if (choice > 0 && choice <= totalItems)
            {
                return choice - 1; // Retorna o índice correto
            }
        }

        // Feedback de erro temporário
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Invalid option, please try again.");
        Console.ResetColor();
        linesWritten++; 
        
        Thread.Sleep(800);
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
