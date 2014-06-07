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
        public ControlServer(string root, IPAddress ip, int port) : base(root, ip, port)
        {

        }

        //public override void Start()
        //{
        //    CreateListener(IPAddress.Any, ServerConfig.ControlPort);
        //    Console.WriteLine("Controlserver has started on port {0}.{1}Waiting for a connection...", ServerConfig.ControlPort, Environment.NewLine);
        //    Program.Logger.WriteMessage(String.Format("{0} has started on port {1}. Waiting for a connection...", ServerName, ServerConfig.WebPort, Environment.NewLine));
        //}
    }
}
