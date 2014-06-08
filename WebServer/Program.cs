using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebServer.Server;
using WebServer.Utilities.Authentication;

namespace WebServer
{
    class Program
    {
        private static Server.Server server;
        private static ControlServer.ControlServer cServer;

        public static Logger.Logger Logger {get; set;}

        static void Main(string[] args)
        {
            // Set on process end event
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            // Set maximum number of threads
            ThreadPool.SetMaxThreads(20, 20);

            ServerConfig.Init();

            Logger = new Logger.Logger();

            server = new Server.Server(ServerConfig.Webroot, IPAddress.Any, ServerConfig.WebPort);
            cServer = new ControlServer.ControlServer(ServerConfig.Controlroot, IPAddress.Any, ServerConfig.ControlPort);

            server.ServerName = "Webserver";
            cServer.ServerName = "Controlserver";

            StartServers();

            Console.Read();

            OnProcessExit();
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
            server.Port = ServerConfig.WebPort;
            cServer.Port = ServerConfig.ControlPort;

            server.Restart();
            cServer.Restart();
        }

        public static void OnProcessExit()
        {
            Console.WriteLine("Closing program");
            StopServers();
        }
    }
}
