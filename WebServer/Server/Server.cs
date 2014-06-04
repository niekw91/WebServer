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
        protected TcpListener tcpListener;
        protected Thread listenThread;
        protected String rootFolder;

        //http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server

        public Server(string root)
        {
            this.rootFolder = root;
            // Set maximum number of threads
            ThreadPool.SetMaxThreads(20, 20);
        }

        public void CreateListener(IPAddress ip, int port)
        {
            this.tcpListener = new TcpListener(ip, port);

            this.listenThread = new Thread(new ThreadStart(Listen));
            this.listenThread.Start();
        }

        public virtual void Start()
        {
            CreateListener(IPAddress.Any, ServerConfig.WebPort);
            Console.WriteLine("Server has started on port {0}.{1}Waiting for a connection...", ServerConfig.WebPort, Environment.NewLine);
        }

        public virtual void Stop()
        {
            this.tcpListener.Stop();
        }

        public virtual void Restart()
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
                //client.ReceiveTimeout = 5;
                //client.SendTimeout = 5;
                // Start timing request
                Stopwatch timer = new Stopwatch();
                timer.Start();

                // HTTPS secure connection stream
                //SslStream stream = new SslStream(client.GetStream(), false);

                Console.WriteLine("----------------");
                Console.WriteLine("A client connected.");
                Console.WriteLine("IP Address: {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                //create a thread to handle communication 
                //with connected client
                RequestHandler rqHandler = new RequestHandler();
                rqHandler.Timer = timer;
                rqHandler.Client = client;
                // Create object array with parameters for request handler
                //object[] parameters = new object[2] { client, rootFolder };
                ThreadPool.QueueUserWorkItem(DoRequest, rqHandler);
            }
        }

        public void DoRequest(object rqh)
        {
            // Evnetueel een web object maken die de request en response afhandeld,
            // aparte class die de requesthandler en response handler bijhoud en aanstuurd

            // Fail met post, als de header is gestuurd maar de data nog niet gaat de request kapot voor dat de data er is.

            // Eventueel response parsen, het content laden weghalen om dit na verwerking pas te doen

            RequestHandler rqHandler = (RequestHandler)rqh;
            ResponseHandler rsHandler = rqHandler.HandleRequest();

            if (rqHandler.IsRequestValid(rqHandler.Request))
            {
                Response response = rsHandler.GetResponseForRequest(rqHandler.Request, rootFolder);

                // Handle request in code
                if (!response.IsErrorResponse())
                {
                    //Route url
                    RouteHandler rtHandler = new RouteHandler(rqHandler.Request, response);
                    rtHandler.RouteUrl();
                }

                rsHandler.ReturnResponse(rqHandler.Client.GetStream(), response);
            }

            
            //rsHandler.HandleResponse(rqHandler.Client, rqHandler.Request, rootFolder);

            rqHandler.Client.Close();
            rqHandler.Timer.Stop();
            Console.WriteLine("Total time: " + rqHandler.Timer.ElapsedMilliseconds + " ms");
            Console.WriteLine("Close connection");
            Console.WriteLine("----------------");
        }

        //public static ServerConfig GetConfig() {
        //    return Server.config;
        //}
    }
}
