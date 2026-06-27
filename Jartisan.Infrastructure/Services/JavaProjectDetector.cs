

using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Services

{
    public class JavaProjectDetector: IProjectDetector
    {

        private readonly string _rootPath = Directory.GetCurrentDirectory();


        public bool ProjectExists()
        {
            bool pomXmlExists = File.Exists(Path.Combine(_rootPath, "pom.xml"));

             bool javaStructureExists = Directory.Exists(Path.Combine(_rootPath, "src", "main", "java"));

            

            return pomXmlExists && javaStructureExists;

        }
    }
}