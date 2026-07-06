using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jartisan.Application.Ports
{
    public interface IFileWriter
    {
        bool Write(string path, string content);
    }
}