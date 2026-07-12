using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Jartisan.Application.Ports;

namespace Jartisan.Infrastructure.Implementations.Maven
{
    public class PomReader : IDependencyReader
    {
        private readonly IProjectDetector _projectDetector;

        public PomReader(IProjectDetector projectDetector)
        {
            _projectDetector = projectDetector ?? throw new ArgumentNullException(nameof(projectDetector));
        }

        public List<string> ListDependencies()
        {
            var pomPath = _projectDetector.PomPath;
            if (!File.Exists(pomPath))
                throw new FileNotFoundException($"pom.xml file not found at {pomPath}.");

            var doc = XDocument.Load(pomPath);
            XNamespace ns = doc.Root.Name.Namespace;

            // Retrieves all dependencies and maps them to a formatted string
            return doc.Descendants(ns + "dependency")
                .Select(d => 
                {
                    var groupId = d.Element(ns + "groupId")?.Value ?? "unknown";
                    var artifactId = d.Element(ns + "artifactId")?.Value ?? "unknown";
                    var version = d.Element(ns + "version")?.Value ?? "unknown";
                    
                    return $"{groupId}:{artifactId} (v{version})";
                })
                .ToList();
        }
    }
}