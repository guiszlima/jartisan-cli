using System.IO;
using Jartisan.Application.Ports;

namespace Jartisan.Application.UseCases.Init
{
    public class InitTemplatesUseCase
    {
        private readonly IFileWriter _writer;

        public InitTemplatesUseCase(IFileWriter writer)
        {
            _writer = writer;
        }

        public void Execute(string templatesFolder)
        {
            // A REGRA INDESTRUTÍVEL: Se a pasta física já existir no disco rígido do usuário,
            // nós encerramos a execução imediatamente. Respeita se o usuário deletou o README!
            if (Directory.Exists(templatesFolder))
            {
                return;
            }

            // O fluxo abaixo só roda se for a PRIMEIRA vez absoluta que o CLI cria a pasta no projeto
            string readmeFile = Path.Combine(templatesFolder, "README.txt");

            string readmeContent = 
                "======================================================================\n" +
                " JARTISAN CLI - CUSTOM TEMPLATES\n" +
                "======================================================================\n\n" +
                "Place your template files (.tpl) in this folder to extend\n" +
                "or override the native generations of the 'jartisan make' command.\n\n" +
                "Files created here have the highest read priority over the binary.\n\n" +
                "Universal variables available for automatic replacement:\n" +
                "  {{package}}      -> The correct Java package (e.g. com.jartisan.controller)\n" +
                "  {{ClassName}}    -> The class name provided in the terminal (e.g. User)\n" +
                "  {{endpointName}} -> The automated kebab-case name for routes (e.g. order-service)\n\n" +
                "Example of creating a new template 'handler.tpl':\n" +
                "  jartisan make handler CreateUser\n";

            // O seu FileWriter assume o controle técnico e faz a pasta nascer de fábrica com o manual
            _writer.Write(readmeFile, readmeContent);
        }
    }
}
