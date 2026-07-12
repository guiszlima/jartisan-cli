namespace Jartisan.Application.Ports
{
    public interface IProjectDetector
    {
        // ADICIONE ESTAS DUAS LINHAS:
        string RootPath { get; }
        string PomPath { get; }

        // Keep your methods that already existed here below:
        bool ProjectExists();
        string GetGroupId();
        string GetArtifactId();
    }
}
