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
                // 1. Scenario: The project already exists
                if (detectUseCase.Execute())
                {
                    Console.WriteLine("The current project is already a Maven project. Do you want to update the jartisan.json file? (y/n)");
                    if (Console.ReadLine()?.Trim().ToLower() == "y")
                    {
                        // Step A: Updates configurations and captures FolderMap from scanner
                        FolderMap map = jsonUseCase.Execute();
                        
                        // Step B: Ensures folder creation only if it doesn't exist on disk
                        templatesUseCase.Execute(map.TemplatesFolder);
                        
                        Console.WriteLine("jartisan.json updated successfully.");
                    }
                    return; // 🛑 Ends command execution here!
                }

                // 2. Scenario: The project does not exist (Creation from scratch)
                Console.Error.WriteLine("The current project is not a Maven project. Would you like to create a Maven project? (y/n)");
                
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    Console.WriteLine("Operation cancelled by the user.");
                    return; // 🛑 Ends execution
                }

                var config = GetUserConfig();

                Console.WriteLine("Creating Maven project...");
                createProjectUseCase.Execute(config); 
                
                // Passo A: Gera e salva o arquivo jartisan.json inicial
                FolderMap novoMap = jsonUseCase.Execute();
                
                // Step B: Creates the customization folder templates.jartisan with README
                templatesUseCase.Execute(novoMap.TemplatesFolder);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error initializing: {ex.Message}");
            }
        }

        /// <summary>
        /// Asks the user for data via terminal. If empty, keeps the record's default.
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
