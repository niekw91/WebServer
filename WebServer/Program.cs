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
        static void Main(string[] args)
        {
            ServerConfig config = new ServerConfig();
            config.Init();
            Console.WriteLine("Config file created");
            Console.Read();
        }
    }
}
