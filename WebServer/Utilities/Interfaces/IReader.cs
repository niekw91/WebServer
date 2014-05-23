using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Utilities.Interfaces
{
    interface IReader
    {
        String ReadFile(String path);
        //Stream ReadFile(String path);
    }
}
