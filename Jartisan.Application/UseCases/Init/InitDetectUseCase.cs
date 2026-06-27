using Jartisan.Application.Ports;
namespace Jartisan.Application.UseCases.Init
{
    public class InitDetectUseCase
    {

        private readonly IProjectDetector _projectDetector;
        public InitDetectUseCase(IProjectDetector projectDetector)
        {
            _projectDetector = projectDetector;
        }

        public bool Execute()
        {
            return _projectDetector.ProjectExists();
        }
       


    }
}