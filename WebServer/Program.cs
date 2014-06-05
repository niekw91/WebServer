using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Server;

namespace WebServer
{
    class Program
    {
        private static Server.Server server;
        private static ControlServer.ControlServer cServer;

        static void Main(string[] args)
        {
            ServerConfig.Init();

            server = new Server.Server(ServerConfig.Webroot);
            cServer = new ControlServer.ControlServer(ServerConfig.Controlroot);

            server.Start();
            cServer.Start();


            Console.Read();
        }

        public static void RestartServers()
        {
            server.Restart();
            cServer.Restart();
        }
    }
}
