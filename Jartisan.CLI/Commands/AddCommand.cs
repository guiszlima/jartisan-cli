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

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AddCommand))]
        [Command("add")]
        [Description("Busca dependências no Maven Central e permite escolher qual adicionar ao pom.xml")]
        public async Task HandleAsync([Argument] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                DisplayError("A query de busca não pode ser vazia.");
                return;
            }

            // 1. Busca os dados (Assíncrono com CancellationToken)
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            Console.Write("Processando busca no Maven Central... ");
            
            var searchTask = _addUseCase.SearchAsync(query, cts.Token);
            var results = await ShowLoadingAnimationAsync(searchTask);

            // 2. Validação dos resultados
            if (results == null || results.Count == 0)
            {
                DisplayError("Nenhuma dependência encontrada para essa query.");
                return;
            }

            // 3. Exibição da lista interativa
            Console.WriteLine($"\nEncontrei {results.Count} resultados:\n");
            for (int i = 0; i < results.Count; i++)
            {
                var dep = results[i];
                Console.WriteLine($" [{i + 1}] {dep.GroupId}:{dep.ArtifactId} (v{dep.Version})");
            }

            // 4. Seleção do usuário
            int selectedIndex = -1;
            while (true)
            {
                Console.Write("\nEscolha o número da dependência (ou 0 para cancelar): ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int choice))
                {
                    if (choice == 0) 
                    {
                        Console.WriteLine("Operação cancelada pelo usuário.");
                        return;
                    }
                    if (choice > 0 && choice <= results.Count)
                    {
                        selectedIndex = choice - 1;
                        break;
                    }
                }
                Console.WriteLine("Opção inválida, tente novamente.");
            }

            // 5. Execução da adição
            var selected = results[selectedIndex];
            try
            {
                bool isAdded = _addUseCase.AddDependency(selected);
    
    Console.ForegroundColor = isAdded ? ConsoleColor.Green : ConsoleColor.Yellow;
    
    Console.WriteLine(isAdded 
        ? $"\n[Sucesso] Adicionado: {selected.GroupId}:{selected.ArtifactId} (v{selected.Version}) ✨" 
        : $"\n[Aviso] A dependência {selected.GroupId}:{selected.ArtifactId} já existe no pom.xml.");
        
    Console.ResetColor();
            }
            catch (Exception ex)
            {
                DisplayError($"Erro ao atualizar pom.xml: {ex.Message}");
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

        private static void DisplayError(string mensagem)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[Erro] {mensagem}");
            Console.ResetColor();
        }
    }
}