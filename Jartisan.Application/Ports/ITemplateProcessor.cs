using System.Collections.Generic;

namespace Jartisan.Application.Ports
{
    public interface ITemplateProcessor
    {
        string Process(string templateName, Dictionary<string, string> replacements, bool isCrud = false);
        string ProcessPackage(string targetPath);
    }
}
