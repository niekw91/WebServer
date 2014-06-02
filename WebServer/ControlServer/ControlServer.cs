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
        public ControlServer() : base()
        {
            
        }

        public override void Start()
        {
            CreateListener(IPAddress.Any, ServerConfig.ControlPort);
            Console.WriteLine("Controlserver has started on port {0}.{1}Waiting for a connection...", ServerConfig.ControlPort, Environment.NewLine);
        }

        public override void Stop()
        {
            this.tcpListener.Stop();
        }

        public override void Restart()
        {
            Stop();
            Start();
        }
    }
}
