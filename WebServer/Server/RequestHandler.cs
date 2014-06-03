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
        public int TimeoutMS { get; set; }
        public TcpClient Client { get; set; }

        private string _formDataContentType = "application/x-www-form-urlencoded";

        public RequestHandler()
        {
            TimeoutMS = 2000;
        }

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
                        if (!line.Contains(":"))
                            break;

                        string[] headerParts = line.Split(':');
                        request.AddHeader(headerParts[0].Trim().ToLower(), headerParts[1].Trim());
                    }
                }

                if (request.GetHeader("content-type") == _formDataContentType)
                {
                    request.FormDataString = reqParts[reqParts.Length - 1];
                }

                return request;
            }
            return null;
        }

        public void HandleRequest(string responseRoot)
        {
            //System.Threading.Thread.Sleep(100);
            //object[] param = (object[])paramaters;
            //TcpClient client = (TcpClient)param[0];
            NetworkStream stream = Client.GetStream();

            // Create response handler
            ResponseHandler rsHandler = new ResponseHandler();

            bool timeout = false;
            // Block until data or timeout
            while (!stream.DataAvailable) { if (Timer.ElapsedMilliseconds > TimeoutMS || !Client.Connected) { timeout = true; break; } }

            if (timeout)
            {
                // Handle timeout response
                rsHandler.HandleTimeout(Client);
            }
            else
            {
                Console.WriteLine(Client.Available);
                // Get request Object
                Request request = RequestToObject(stream, Client.Available);

                // Check if request is valid
                if (IsRequestValid(request))
                {
                    // Set full url
                    request.FullUrl = request.Url;
                    // Parse request values
                    ParseRequestValues(request);

                    // Set root folder
                    //String root = (String)param[1];

                    foreach (KeyValuePair<String, String> kvp in request.Values)
                    {
                        Console.WriteLine(String.Format("key: {0} value {1}", kvp.Key, kvp.Value));
                    }

                    Console.WriteLine("Start handler");
                    rsHandler.HandleResponse(Client, request, responseRoot);
                }
                else
                {
                    Console.WriteLine("Invalid request");
                    rsHandler.HandleBadRequest(Client);
                }
            }
           
            // Stop connection and stop total handle time
            Client.Close();
            Timer.Stop();
            Console.WriteLine("Total time: " + Timer.ElapsedMilliseconds + " ms");
            Console.WriteLine("Close connection");
            Console.WriteLine("----------------");
        }

        private Request RequestToObject(NetworkStream stream, int length)
        {
            // Read content from stream and Parse to Request
            byte[] bytes = new Byte[length];

            stream.Read(bytes, 0, bytes.Length);

            //translate bytes of request to string
            String data = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(data);
            return ParseStringToRequest(data);
        }

        private bool IsRequestValid(Request request)
        {
            return (request != null && (request.Method == "POST" || request.Method == "GET"));
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
            if (request.GetHeader("content-type") == _formDataContentType)
            {
                request.Values = ParseFormData(request.FormDataString);
            }
        }

        private void ParseGetValues(Request request)
        {
            string[] urlParts = request.Url.Split('?');
            request.Url = urlParts[0];
            if (urlParts.Length > 1)
            {
                request.Values = ParseFormData(urlParts[1]);
            }
        }

        private Dictionary<string, string> ParseFormData(string formDataStr)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            string[] parameters = formDataStr.Split('&');
            for (int i = 0; i < parameters.Length; i++)
            {
                string[] keyValues = parameters[i].Split('=');
                if (keyValues.Length == 2)
                {
                    values.Add(keyValues[0].ToLower(), keyValues[1]);
                }
            }
            return values;
        }
    }
}
