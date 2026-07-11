using System;
using System.Diagnostics.CodeAnalysis; 
using ConsoleAppFramework;
using Jartisan.Application.UseCases.Init;
using Jartisan.Domain.Entities;

namespace Jartisan.CLI.Commands
{
    public class InitCommand(
        InitDetectUseCase detectUseCase, 
        InitProjectUseCase createProjectUseCase, 
        InitJsonUseCase jsonUseCase,
        InitTemplatesUseCase templatesUseCase) 
    {
        
        
        /// <summary>
        /// Initializes a new Maven project interactively or updates existing configurations.
        /// </summary>
        [Command("init")]
        public void Execute()
        {
            try
            {
                // 1. Cenário: O projeto já existe
                if (detectUseCase.Execute())
                {
                    Console.WriteLine("The current project is already a Maven project. Do you want to update the jartisan.json file? (y/n)");
                    if (Console.ReadLine()?.Trim().ToLower() == "y")
                    {
                        // Passo A: Atualiza as configurações e captura o FolderMap do scanner
                        FolderMap map = jsonUseCase.Execute();
                        
                        // Passo B: Garante a criação da pasta apenas se ela não existir no HD
                        templatesUseCase.Execute(map.TemplatesFolder);
                        
                        Console.WriteLine("jartisan.json updated successfully.");
                    }
                    return; // 🛑 Encerra a execução do comando aqui!
                }

                // 2. Cenário: O projeto não existe (Criação do zero)
                Console.Error.WriteLine("The current project is not a Maven project. Would you like to create a Maven project? (y/n)");
                
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    Console.WriteLine("Operation cancelled by the user.");
                    return; // 🛑 Encerra a execução
                }

                var config = GetUserConfig();

                Console.WriteLine("Creating Maven project...");
                createProjectUseCase.Execute(config); 
                
                // Passo A: Gera e salva o arquivo jartisan.json inicial
                FolderMap novoMap = jsonUseCase.Execute();
                
                // Passo B: Cria a pasta de customização templates.jartisan com o README
                templatesUseCase.Execute(novoMap.TemplatesFolder);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error initializing: {ex.Message}");
            }
        }

        /// <summary>
        /// Pergunta os dados ao usuário pelo terminal. Se forem vazios, mantém o padrão do record.
        /// </summary>
        private JavaProjectConfig GetUserConfig()
        {
            var configPadrao = new JavaProjectConfig();
            
            Console.Write($"Enter the GroupId [{configPadrao.GroupId}]: ");
            string? inputGroupId = Console.ReadLine()?.Trim();
            
            Console.Write("Enter the ArtifactId [Current folder name]: ");
            string? inputArtifactId = Console.ReadLine()?.Trim();
            
            Console.Write($"Enter the Version [{configPadrao.Version}]: ");
            string? inputVersion = Console.ReadLine()?.Trim();

            return new JavaProjectConfig
            {
                GroupId = string.IsNullOrWhiteSpace(inputGroupId) ? configPadrao.GroupId : inputGroupId,
                ArtifactId = string.IsNullOrWhiteSpace(inputArtifactId) ? null : inputArtifactId, 
                Version = string.IsNullOrWhiteSpace(inputVersion) ? configPadrao.Version : inputVersion
            };
        }
    }
}
