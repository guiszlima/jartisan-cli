

using Jartisan.Application.Ports;
using Jartisan.Domain.Entities;

namespace Jartisan.Application.UseCases.Init

{
    public class InitProjectUseCase
    {
        
            private readonly IProjectFactory _projectFactory;
            public InitProjectUseCase(IProjectFactory projectFactory)
            {
                _projectFactory = projectFactory;
            }
    
            public void Execute(JavaProjectConfig? config = null)
            {
                _projectFactory.CreateProject(config);
            }   

    }
}