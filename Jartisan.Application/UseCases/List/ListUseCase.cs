using System;
using System.Collections.Generic;
using Jartisan.Application.Ports;

namespace Jartisan.Application.UseCases.List
{
    public class ListUseCase
    {
        private readonly IDependencyReader _reader;

        public ListUseCase(IDependencyReader reader)
        {
            _reader = reader;
        }

        public List<string> Execute()
        {
            // The UseCase acts as an orchestrator. 
            // It calls the port (IDependencyReader) to get data.
            return _reader.ListDependencies();
        }
    }
}