using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class Request : WebObject
    {
        public string Method { get; set; }
        public string Url { get; set; }
        //public string HTTPVersion { get; set; }
        //public Dictionary<string, string> Headers { get { return _headers; } }

        //private Dictionary<string, string> _headers;

    }
}
