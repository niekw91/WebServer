using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class Server
    {
        private static readonly String CERTIFICATE_PATH = @"config\webserver.cer";
        public static X509Certificate ServerCertificate { get; set; }

        protected TcpListener tcpListener;
        protected Thread listenThread;
        protected String rootFolder;

        private bool running;
        public string RestartId { get; set; }

        private Dictionary<String, TcpClient> clients;

        //http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server

        public Server(string root)
        {
            // Create certificate from file
            ServerCertificate = X509Certificate.CreateFromCertFile(CERTIFICATE_PATH);
            RestartId = "";
            // Create client dictionary
            clients = new Dictionary<string, TcpClient>();
            this.rootFolder = root;
            
        }

        public void CreateListener(IPAddress ip, int port)
        {
            this.tcpListener = new TcpListener(ip, port);

            running = true;

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
            Console.WriteLine("Stopping server");
            running = false;
            foreach (KeyValuePair<String, TcpClient> kvp in clients)
            {
                kvp.Value.Close();
            }
            while (!this.tcpListener.Pending())
            {
                this.tcpListener.Stop();
                break;
            }
        }

        public virtual void Restart()
        {
            Stop();
            Start();
        }

        private void Listen()
        {
            this.tcpListener.Start();

            while (running)
            {
                //blocks until a client has connected to the server
                try
                {
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    //CustomTcpClient custom = new CustomTcpClient();
                    //custom.Client = client;
                    //custom.GenerateId();

                    //client.ReceiveTimeout = 1;
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
                catch (SocketException se) { Console.WriteLine(se.Message); }
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

            String id = null;

            if (rqHandler.IsRequestValid(rqHandler.Request))
            {
                rqHandler.GenerateId();
                clients.Add(rqHandler.Request.Id, rqHandler.Client);

                Response response = rsHandler.GetResponseForRequest(rqHandler.Request, rootFolder);

                // Handle request in code
                if (!response.IsErrorResponse())
                {
                    //Route url
                    RouteHandler rtHandler = new RouteHandler(rqHandler.Request, response);
                    rtHandler.RouteUrl(this);
                }

                rsHandler.ReturnResponse(rqHandler.Client.GetStream(), response);

                lock (clients)
                {
                    clients.Remove(rqHandler.Request.Id);
                }
                id = rqHandler.Request.Id;
            }

            
            //rsHandler.HandleResponse(rqHandler.Client, rqHandler.Request, rootFolder)

            rqHandler.Client.Close();
            rqHandler.Timer.Stop();
            Console.WriteLine("Total time: " + rqHandler.Timer.ElapsedMilliseconds + " ms");
            Console.WriteLine("Close connection");
            Console.WriteLine("----------------");

            RestartOnId(id);
        }
        
        public void RestartOnId(string id)
        {
            if (id == RestartId) { Program.RestartServers(); RestartId = ""; }
        }
    }
}
