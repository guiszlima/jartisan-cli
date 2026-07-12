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

            // Keeps original comments and whitespace
            var doc = XDocument.Load(pomPath, LoadOptions.PreserveWhitespace);
            
            var root = doc.Elements().FirstOrDefault(e => e.Name.LocalName == "project");
            if (root == null) throw new Exception("The <project> tag was not found in pom.xml.");
            
            XNamespace activeNs = root.Name.Namespace;

            // Idempotency verification
            if (DependencyExists(doc, dependency)) return false;

            // 3. Garante o bloco <dependencies>
            var dependencies = root.Elements().FirstOrDefault(e => e.Name.LocalName == "dependencies");
            if (dependencies == null)
            {
                dependencies = new XElement(activeNs + "dependencies");
                root.Add(new XText("\n    "), dependencies, new XText("\n"));
            }

            // --- Formatting logic ---
            // Attempts to detect the indentation of the last element (if any)
            string indent = "    "; // Default of 4 spaces
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
            
            // Ensures that the closing dependencies tag also has a new line
            if (dependencies.LastNode is not XText)
            {
                dependencies.Add(new XText("\n"));
            }

            // 5. Atomic write
            string tempPath = pomPath + ".tmp";
            doc.Save(tempPath);
            File.Move(tempPath, pomPath, overwrite: true);
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