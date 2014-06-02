using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // Set maximum number of threads
            ThreadPool.SetMaxThreads(20, 20);

            Server.config = config;
        }

        public void CreateListener(IPAddress ip, int port)
        {
            this.tcpListener = new TcpListener(ip, port);

            this.listenThread = new Thread(new ThreadStart(Listen));
            this.listenThread.Start();
        }

        public void Start()
        {
            CreateListener(IPAddress.Any, config.WebPort);
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
                client.ReceiveTimeout = 5;
                client.SendTimeout = 5;
                // Start timing request
                Stopwatch timer = new Stopwatch();
                timer.Start();
 
                Console.WriteLine("----------------");
                Console.WriteLine("A client connected.");
                Console.WriteLine("IP Address: {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                //create a thread to handle communication 
                //with connected client
                RequestHandler rqHandler = new RequestHandler();
                rqHandler.Timer = timer;
                ThreadPool.QueueUserWorkItem(rqHandler.HandleRequest, client);
                //Thread clientThread = new Thread(new ParameterizedThreadStart(rqHandler.HandleRequest));
                //clientThread.Start(client);
            }
        }

        public static ServerConfig GetConfig() {
            return Server.config;
        }
    }
}
