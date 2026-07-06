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
        InitTemplatesUseCase templatesUseCase) // ADICIONADO: Injeção do novo Use Case focado em SRP            
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(InitCommand))]
        [Command("init")]
        public void Execute()
        {
            try
            {
                // 1. Cenário: O projeto já existe
                if (detectUseCase.Execute())
                {
                    Console.WriteLine("O projeto atual já é um projeto Maven. Deseja atualizar o arquivo jartisan.json? (s/n)");
                    if (Console.ReadLine()?.Trim().ToLower() == "s")
                    {
                        // Passo A: Atualiza as configurações e captura o FolderMap do scanner
                        FolderMap map = jsonUseCase.Execute();
                        
                        // Passo B: Garante a criação da pasta apenas se ela não existir no HD
                        templatesUseCase.Execute(map.TemplatesFolder);
                        
                        Console.WriteLine("Arquivo jartisan.json atualizado com sucesso.");
                    }
                    return; // 🛑 Encerra a execução do comando aqui!
                }

                // 2. Cenário: O projeto não existe (Criação do zero)
                Console.Error.WriteLine("O projeto atual não é um projeto Maven. Gostaria de criar um projeto Maven? (s/n)");
                
                if (Console.ReadLine()?.Trim().ToLower() != "s")
                {
                    Console.WriteLine("Operação cancelada pelo usuário.");
                    return; // 🛑 Encerra a execução
                }

                var config = GetUserConfig();

                Console.WriteLine("Criando projeto Maven...");
                createProjectUseCase.Execute(config); 
                
                // Passo A: Gera e salva o arquivo jartisan.json inicial
                FolderMap novoMap = jsonUseCase.Execute();
                
                // Passo B: Cria a pasta de customização templates.jartisan com o README
                templatesUseCase.Execute(novoMap.TemplatesFolder);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao inicializar: {ex.Message}");
            }
        }

        /// <summary>
        /// Pergunta os dados ao usuário pelo terminal. Se forem vazios, mantém o padrão do record.
        /// </summary>
        private JavaProjectConfig GetUserConfig()
        {
            var configPadrao = new JavaProjectConfig();
            
            Console.Write($"Informe o GroupId [{configPadrao.GroupId}]: ");
            string? inputGroupId = Console.ReadLine()?.Trim();
            
            Console.Write("Informe o ArtifactId [Nome da pasta atual]: ");
            string? inputArtifactId = Console.ReadLine()?.Trim();
            
            Console.Write($"Informe a Versão [{configPadrao.Version}]: ");
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
