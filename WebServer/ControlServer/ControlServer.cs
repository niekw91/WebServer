using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebServer.Server;

namespace WebServer.ControlServer
{
    class ControlServer : Server.Server
    {
        public ControlServer(string root, IPAddress ip, int port) : base(root, ip, port) { }
    }
}
