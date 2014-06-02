using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class RequestHandler
    {
        public Stopwatch Timer { get; set; }
        public Request ParseStringToRequest(string requestHeaders)
        {
            string[] reqParts = requestHeaders.Split(Environment.NewLine.ToCharArray());
            if (reqParts.Length > 0 && !String.IsNullOrEmpty(reqParts[0]))
            {
                string firstLine = reqParts[0];
                reqParts[0] = null;
                string[] firstLineParts = firstLine.Split(' ');
                Request request = new Request();

                request.Method = firstLineParts[0];
                request.Url = firstLineParts[1];
                request.HTTPVersion = firstLineParts[2];

                foreach (string line in reqParts)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        string[] headerParts = line.Split(':');
                        request.AddHeader(headerParts[0].Trim(), headerParts[1].Trim());
                    }
                }

                return request;
            }
            return null;
        }

        public void HandleRequest(object clientReq)
        {
            TcpClient client = (TcpClient)clientReq;
            NetworkStream stream = client.GetStream();

            //byte[] bytes = new byte[4096];
            //int bytesRead;

            //while (true)
            //{
            //    bytesRead = 0;

            //    try
            //    {
            //        //blocks until a client sends a message
            //        bytesRead = clientStream.Read(bytes, 0, 4096);
            //    }
            //    catch
            //    {
            //        //a socket error has occured
            //        break;
            //    }

            //    if (bytesRead == 0)
            //    {
            //        //the client has disconnected from the server
            //        break;
            //    }

            //    //message has successfully been received
            //    //ASCIIEncoding encoder = new ASCIIEncoding();
            //    //Console.WriteLine(encoder.GetString(bytes, 0, bytesRead));
            
            //}
            byte[] bytes = new Byte[client.Available];

            stream.Read(bytes, 0, bytes.Length);

            //translate bytes of request to string
            String data = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(data);
            Request request = ParseStringToRequest(data);
            // Create response handler
            ResponseHandler rsHandler = new ResponseHandler();
            //new Regex("^GET").IsMatch(data)
            if (request != null && (request.Method == "POST" || request.Method == "GET"))
            {
                // Set full url
                request.FullUrl = request.Url;
                // Parse request values
                ParseRequestValues(request);

                foreach (KeyValuePair<String, String> kvp in request.Values)
                {
                    Console.WriteLine(String.Format("key: {0} value {1}", kvp.Key, kvp.Value));
                }

                Console.WriteLine("Start handler");
                rsHandler.HandleResponse(client, request);
            }
            else
            {
                Console.WriteLine("Invalid request");
                rsHandler.HandleBadRequest(client);
            }
           

            client.Close();
            Timer.Stop();
            Console.WriteLine("Total time: " + Timer.ElapsedMilliseconds + " ms");
            Console.WriteLine("Close connection");
            Console.WriteLine("----------------");
        }


        private void ParseRequestValues(Request request)
        {
            switch (request.Method)
            {
                case "GET":
                    ParseGetValues(request);
                    break;
                case "POST":
                    ParsePostValues(request);
                    break;
            }

        }

        private void ParsePostValues(Request request)
        {

        }

        private void ParseGetValues(Request request)
        {
            string[] urlParts = request.Url.Split('?');
            request.Url = urlParts[0];
            if (urlParts.Length > 1)
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                string[] parameters = urlParts[1].Split('&');
                for (int i = 0; i < parameters.Length; i++)
                {
                    string[] keyValues = parameters[i].Split('=');
                    if (keyValues.Length == 2)
                    {
                        values.Add(keyValues[0].ToLower(), keyValues[1]);
                    }
                }
                request.Values = values;
            }
        }
    }
}
