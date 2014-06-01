using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class Server
    {
        private static ServerConfig config;

        private TcpListener tcpListener;
        private Thread listenThread;

        //http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server

        public Server(ServerConfig config)
        {
            Server.config = config;
        }

        public void Start()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, config.WebPort);

            this.listenThread = new Thread(new ThreadStart(Listen));
            this.listenThread.Start();

            Console.WriteLine("Server has started on port 8080.{0}Waiting for a connection...", Environment.NewLine);
        }

        public void Stop()
        {
            this.tcpListener.Stop();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        private void Listen()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();
                Console.WriteLine("----------------");
                Console.WriteLine("A client connected.");

                //create a thread to handle communication 
                //with connected client
                RequestHandler rqHandler = new RequestHandler();
                Thread clientThread = new Thread(new ParameterizedThreadStart(rqHandler.HandleRequest));
                clientThread.Start(client);
            }
        }

        public static ServerConfig GetConfig() {
            return Server.config;
        }
    }
}
