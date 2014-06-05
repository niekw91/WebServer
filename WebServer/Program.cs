using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Server;

namespace WebServer
{
    class Program
    {
        private static Server.Server server;
        private static ControlServer.ControlServer cServer;

        public static Logger.Logger Logger;

        static void Main(string[] args)
        {
            // Set maximum number of threads
            ThreadPool.SetMaxThreads(20, 20);

            ServerConfig.Init();

            Logger = new Logger.Logger();

            server = new Server.Server(ServerConfig.Webroot);
            cServer = new ControlServer.ControlServer(ServerConfig.Controlroot);

            StartServers();

            Console.Read();
        }

        public static void StartServers()
        {
            server.Start();
            cServer.Start();
        }

        public static void StopServers()
        {
            server.Stop();
            cServer.Stop();
        }

        public static void RestartServers()
        {
            server.Restart();
            cServer.Restart();
        }
    }
}
