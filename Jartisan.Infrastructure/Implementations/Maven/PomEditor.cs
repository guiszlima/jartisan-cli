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
        private readonly string _pomPath;

        public PomEditor()
        {
            _pomPath = Path.Combine(Directory.GetCurrentDirectory(), "pom.xml");
        }

        public bool AddDependency(DependencyInfo dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));

            if (!File.Exists(_pomPath))
                throw new FileNotFoundException("pom.xml was not found in the current directory.");

            // Mantém comentários e espaços em branco originais
            var doc = XDocument.Load(_pomPath, LoadOptions.PreserveWhitespace);
            
            var root = doc.Elements().FirstOrDefault(e => e.Name.LocalName == "project");
            if (root == null) throw new Exception("The <project> tag was not found in pom.xml.");
            
            XNamespace activeNs = root.Name.Namespace;

            // Verificação de Idempotência
            if (DependencyExists(doc, dependency)) return false;

            // 3. Garante o bloco <dependencies>
            var dependencies = root.Elements().FirstOrDefault(e => e.Name.LocalName == "dependencies");
            if (dependencies == null)
            {
                dependencies = new XElement(activeNs + "dependencies");
                root.Add(new XText("\n    "), dependencies, new XText("\n"));
            }

            // --- Lógica de Formatação ---
            // Tenta detectar a indentação do último elemento (se houver)
            string indent = "    "; // Default de 4 espaços
            var lastChild = dependencies.Elements().LastOrDefault();
            
            // Adiciona quebra de linha antes do elemento
            dependencies.Add(new XText("\n" + indent));

            // Cria o elemento
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
            
            // Garante que o fechamento da tag dependencies também tenha nova linha
            if (dependencies.LastNode is not XText)
            {
                dependencies.Add(new XText("\n"));
            }

            // 5. Escrita Atômica
            string tempPath = _pomPath + ".tmp";
            doc.Save(tempPath);
            File.Move(tempPath, _pomPath, overwrite: true);
            return true;
        }

        private bool DependencyExists(XDocument doc, DependencyInfo dependency)
        {
            return doc.Descendants().Where(d => d.Name.LocalName == "dependency")
                .Any(d => 
                    d.Elements().FirstOrDefault(e => e.Name.LocalName == "groupId")?.Value == dependency.GroupId &&
                    d.Elements().FirstOrDefault(e => e.Name.LocalName == "artifactId")?.Value == dependency.ArtifactId
                );
        }
    }
}