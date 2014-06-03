﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Server;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConfig.Init();

            Server.Server server = new Server.Server(ServerConfig.Webroot);
            ControlServer.ControlServer cServer = new ControlServer.ControlServer(ServerConfig.Controlroot);

            server.Start();
            //cServer.Start();


            Console.Read();
        }
    }
}
