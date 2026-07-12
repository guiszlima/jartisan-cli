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
            // THE INDESTRUCTIBLE RULE: If the physical folder already exists on the user's disk,
            // we terminate execution immediately. Respects if the user deleted the README!
            if (Directory.Exists(templatesFolder))
            {
                return;
            }

            // The flow below only runs if it's the FIRST absolute time the CLI creates the folder in the project
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

            // Your FileWriter takes technical control and makes the folder come out of the factory with the manual
            _writer.Write(readmeFile, readmeContent);
        }
    }
}
