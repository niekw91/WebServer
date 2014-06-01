using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class RequestHandler
    {

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

            //new Regex("^GET").IsMatch(data)
            if (request != null)
            {
                Console.WriteLine("Start handler");
                ResponseHandler rsHandler = new ResponseHandler();
                rsHandler.HandleResponse(client, request);
            }
            else
            {
                Console.WriteLine("Invalid request");
            }
           

            client.Close();
            Console.WriteLine("Close connection");
            Console.WriteLine("----------------");
        }
    }
}
