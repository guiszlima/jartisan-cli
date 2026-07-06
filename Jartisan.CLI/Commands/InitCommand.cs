using ConsoleAppFramework;
using Jartisan.Application.UseCases.Init;
using Jartisan.Domain.Entities;

using System.Diagnostics.CodeAnalysis; 

namespace Jartisan.CLI.Commands

{

    
    public class InitCommand
    (
        InitDetectUseCase detectUseCase, 
        InitProjectUseCase createProjectUseCase, 
        InitJsonUseCase jsonUseCase)             
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
                jsonUseCase.Execute();
                Console.WriteLine("Arquivo jartisan.json atualizado com sucesso.");
            }
            return; // 🛑 Encerra a execução do comando aqui!
        }

        // 2. Cenário: O projeto não existe 
        Console.Error.WriteLine("O projeto atual não é um projeto Maven. Gostaria de criar um projeto Maven? (s/n)");
        
        if (Console.ReadLine()?.Trim().ToLower() != "s")
        {
            Console.WriteLine("Operação cancelada pelo usuário.");
            return; // 🛑 Encerra a execução
        }

       
        var config = GetUserConfig();

        Console.WriteLine("Criando projeto Maven...");
        createProjectUseCase.Execute(config); 
        jsonUseCase.Execute();
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
                // Se o usuário apertou Enter (vazio), usa o valor padrão do Record
                GroupId = string.IsNullOrWhiteSpace(inputGroupId) ? configPadrao.GroupId : inputGroupId,
                ArtifactId = string.IsNullOrWhiteSpace(inputArtifactId) ? null : inputArtifactId, 
                Version = string.IsNullOrWhiteSpace(inputVersion) ? configPadrao.Version : inputVersion
            };
        }
    }
}
