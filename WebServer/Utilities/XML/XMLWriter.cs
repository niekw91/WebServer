using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WebServer.Utilities.Interfaces;

namespace WebServer.Utilities
{
    class XMLWriter : IWriter
    {
        public XMLWriter()
        {

        }
        public void WriteFile(string path, string content)
        {
            throw new NotImplementedException();
        }
        public void WriteLine(string path, string line)
        {
            throw new NotImplementedException();
        }
    }
}
