using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Jartisan.Domain.Models; 
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Implementations.Maven
{
    public class PomEditor : IDependencyEditor
    {
        private readonly IProjectDetector _projectDetector;

        public PomEditor(IProjectDetector projectDetector)
        {
            _projectDetector = projectDetector ?? throw new ArgumentNullException(nameof(projectDetector));
        }

        public bool AddDependency(DependencyInfo dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));

            var pomPath = _projectDetector.PomPath;
            if (!File.Exists(pomPath))
                throw new FileNotFoundException($"pom.xml was not found at {pomPath}.");

            // Mantém os comentários e espaçamentos originais do arquivo do usuário
            var doc = XDocument.Load(pomPath, LoadOptions.PreserveWhitespace);
            
            var root = doc.Elements().FirstOrDefault(e => e.Name.LocalName == "project");
            if (root == null) throw new Exception("The <project> tag was not found in pom.xml.");
            
            XNamespace activeNs = root.Name.Namespace;

            // 1. Busca por uma dependência existente com o mesmo GroupId e ArtifactId
            var existingDependency = doc.Descendants()
                .Where(d => d.Name.LocalName == "dependency")
                .FirstOrDefault(d => 
                    d.Elements().FirstOrDefault(e => e.Name.LocalName == "groupId")?.Value == dependency.GroupId &&
                    d.Elements().FirstOrDefault(e => e.Name.LocalName == "artifactId")?.Value == dependency.ArtifactId
                );

            if (existingDependency != null)
            {
                var versionElement = existingDependency.Elements().FirstOrDefault(e => e.Name.LocalName == "version");
                
                // Idempotência: Se já for a mesma versão, não faz nada e retorna false
                if (versionElement?.Value == dependency.Version) return false;

                if (versionElement != null)
                {
                    // Cenário A: A tag <version> já existe, apenas substitui o valor interno
                    versionElement.Value = dependency.Version;
                    Console.ForegroundColor = ConsoleColor.Yellow;
              Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[Warn] {dependency.ArtifactId} already exists, upgrading to v{dependency.Version}... 🔄");
                Console.ResetColor();

                }
                else
                {
                    // Cenário B: A dependência existe mas não tem versão explícita (herdada de BOM/Parent)
                    // Injeta a tag <version> logo após o <artifactId> para manter a ordem idiomática do Maven
                    var artifactElement = existingDependency.Elements().FirstOrDefault(e => e.Name.LocalName == "artifactId");
                    if (artifactElement != null)
                    {
                        var baseIndent = "    "; // Alinhamento padrão da CLI
                        artifactElement.AddAfterSelf(
                            new XText("\n" + baseIndent + "    "),
                            new XElement(activeNs + "version", dependency.Version)
                        );
                    }
                    else
                    {
                        // Fallback de segurança caso a estrutura esteja atípica
                        existingDependency.Add(new XElement(activeNs + "version", dependency.Version));
                    }
                }
                
                // Salva o arquivo modificado de forma atômica
                SaveAtomic(doc, pomPath);
                return true; 
            }

            // 2. Garante a existência do bloco pai <dependencies>
            var dependencies = root.Elements().FirstOrDefault(e => e.Name.LocalName == "dependencies");
            if (dependencies == null)
            {
                dependencies = new XElement(activeNs + "dependencies");
                root.Add(new XText("\n    "), dependencies, new XText("\n"));
            }

            // --- Lógica de Formatação para Nova Inserção ---
            string indent = "    "; 
            dependencies.Add(new XText("\n" + indent));

            var newDependency = new XElement(activeNs + "dependency",
                new XText("\n" + indent + "    "),
                new XElement(activeNs + "groupId", dependency.GroupId),
                new XText("\n" + indent + "    "),
                new XElement(activeNs + "artifactId", dependency.ArtifactId),
                new XText("\n" + indent + "    "),
                new XElement(activeNs + "version", dependency.Version),
                new XText("\n" + indent)
            );

            dependencies.Add(newDependency);
            
            // Garante que o fechamento da tag </dependencies> quebre a linha corretamente
            if (dependencies.LastNode is not XText)
            {
                dependencies.Add(new XText("\n" + indent));
            }

            // 3. Salva de forma atômica o novo nó adicionado
            SaveAtomic(doc, pomPath);
            return true;
        }

        private void SaveAtomic(XDocument doc, string pomPath)
        {
            string tempPath = pomPath + ".tmp";
            doc.Save(tempPath);
            File.Move(tempPath, pomPath, overwrite: true);
        }
    }
}
