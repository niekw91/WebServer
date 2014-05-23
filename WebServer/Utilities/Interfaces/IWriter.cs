using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Utilities.Interfaces
{
    interface IWriter
    {
        void WriteFile(String path, String content);
        void WriteLine(String path, String line);
    }
}
