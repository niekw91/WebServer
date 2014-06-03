using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class ResponseHandler
    {

        public byte[] GetFileForUrlPath(string url)
        {
            string path = ServerConfig.Webroot + url;
            if (url.EndsWith("/") || !url.Contains('.'))
            {
                path += ServerConfig.DefaultPage.Split(';')[0];
            }
            try
            {
                return File.ReadAllBytes(path);
            } catch(Exception ex) {
                return null;
            }
        }

        public string ParseUrlToPath(string url)
        {
            string path = ServerConfig.Webroot + url;

            return path;
        }

        public void HandleBadRequest(TcpClient client)
        {
            Response response = new Response();
            response.StatusCode = 400;
            response.StatusMessage = "Bad Request";
            response.AddHeader("Connection", "close");
            response.AddHeader("Content-type", "text/html");
            response.AddHeader("Cache-Control", "no-cache, must-revalidate");
            response.AddHeader("Server", ServerConfig.Name);

            ReturnResponse(client.GetStream(), response);
        }

        public void HandleTimeout(TcpClient client) 
        {
            Response response = new Response();
            response.StatusCode = 408;
            response.StatusMessage = "Request Timeout";
            response.AddHeader("Connection", "close");
            response.AddHeader("Content-type", "text/html");
            response.AddHeader("Cache-Control", "no-cache, must-revalidate");
            response.AddHeader("Server", ServerConfig.Name);
            response.Content = GetErrorPage(response.StatusCode);

            ReturnResponse(client.GetStream(), response);
        }

        public void HandleResponse(TcpClient client, Request request, String root)
        {
            NetworkStream stream = client.GetStream();
            //@"C:\Users\Remi\Documents\GitHub\WebServer\WebServer\index.html"
            Response response = new Response(request);
            response.StatusCode = 200;
            response.StatusMessage = "OK";

            if (!ParseValidUrl(request.Url))
            {
                request.Url = FixUrl(request.Url);
                request.Url = request.Url + GetDefaultPage(request);
            }

            response.Path = root + request.Url;
            if (File.Exists(response.Path))
            {
                response.Content = File.ReadAllBytes(response.Path);
            }

            if (response.Content != null)
            {
                response.AddHeader("Accept-Ranges", "bytes");
                response.AddHeader("Connection", "keep-alive");
                response.AddHeader("Content-Length", response.Content.Length.ToString());
                response.AddHeader("Content-Type", response.ContentType);
                response.AddHeader("Cache-Control", "none");
                response.AddHeader("Date", response.CurrentDate);
                response.AddHeader("Keep-Alive", "timeout=5, max=100");
                response.AddHeader("Last-Modified", response.CurrentDate);
            } else {
                response.StatusCode = 404;
                response.StatusMessage = "Not Found";
                response.AddHeader("Connection", "close");
                response.AddHeader("Content-type", "text/html");
                response.AddHeader("Cache-Control", "no-cache, must-revalidate");
                response.Content = GetErrorPage(response.StatusCode);
            }
            response.AddHeader("Server", ServerConfig.Name);

            //response.Content = GetFileForUrlPath(response.Path);
            //string head = null;

            //if (response.Content != null && !request.Url.EndsWith(".png"))
            //{
            //    head = "HTTP/1.1 100 Continue" + Environment.NewLine
            //        + String.Format("{0:ddd dd MMM yy HH:mm:ss} GMT", DateTime.Now) + Environment.NewLine
            //        + AddHeader("Accept-Ranges", "bytes")
            //        + AddHeader("Connection", "Keep-Alive")
            //        + AddHeader("Content-Length", content.Length.ToString())
            //        + AddHeader("Content-Type", "text/html")
            //        + AddHeader("Cache-Control", "none")
            //        + AddHeader("Date", DateTime.Now.ToString())
            //        + AddHeader("Keep-Alive", "timeout=5, max=100")
            //        + AddHeader("Last-Modified", DateTime.Now.ToString())
            //        + AddHeader("Server", "Homemade 0.1");
            //} else if(response.Content != null && request.Url.EndsWith(".png")) {
            //    head = "HTTP/1.1 100 Continue" + Environment.NewLine
            //        + String.Format("{0:ddd dd MMM yy HH:mm:ss} GMT", DateTime.Now) + Environment.NewLine
            //        + AddHeader("Accept-Ranges", "bytes")
            //        + AddHeader("Connection", "Keep-Alive")
            //        + AddHeader("Content-Length", content.Length.ToString())
            //        + AddHeader("Content-Type", "image/png")
            //        + AddHeader("Date", DateTime.Now.ToString())
            //        + AddHeader("Keep-Alive", "timeout=5, max=100")
            //        + AddHeader("Last-Modified", DateTime.Now.ToString())
            //        + AddHeader("Mime-Type", "image/png")
            //        + AddHeader("Server", "Homemade 0.1");
            //} else
            //{
            //    head = "HTTP/1.1 404 Not Found" + Environment.NewLine
            //        + String.Format("{0:ddd dd MMM yy HH:mm:ss} GMT", DateTime.Now) + Environment.NewLine
            //        + AddHeader("Server", "Homemade 0.1");
            //}
            //byte[] header = Encoding.UTF8.GetBytes(head);
            
            Console.WriteLine(response.GetHeadersAsString());

            ReturnResponse(stream, response);
        }

        public void ReturnResponse(NetworkStream stream, Response response)
        {
            byte[] _response = (response.Content != null) ? response.GetResponseHeaderAsByteArray().Concat(response.Content).ToArray() : response.GetResponseHeaderAsByteArray();
            stream.Write(_response, 0, _response.Length);
            stream.Flush();
            stream.Close();
        }

        public Byte[] GetErrorPage(int statusCode)
        {
            String path = @"webroot\www\errors\{0}.html";
            if (File.Exists(String.Format(path, statusCode)))
            {
                return File.ReadAllBytes(String.Format(path, statusCode));
            }
            return null;
        }

        private bool ParseValidUrl(string url)
        {
            string[] urlParts = url.Split('/');
            return urlParts[urlParts.Length - 1].Contains('.');
        }

        private string FixUrl(string url)
        {
            string[] urlParts = url.Split('/');
            if (String.IsNullOrEmpty(urlParts[urlParts.Length - 1]))
            {
                url += "/";
            }
            return url;
        }

        private string GetDefaultPage(Request request)
        {
            string[] defaultPages = ServerConfig.DefaultPage.Split(';');
            foreach (string defPage in defaultPages)
            {
                if (File.Exists(ServerConfig.Webroot + request.Url + defPage))
                    return defPage;
            }
            return null;
        }

        //public string AddHeader(string header, string value)
        //{
        //    return header + ": " + value + Environment.NewLine;
        //}

        //public string GetContentType(string url)
        //{
        //    return (url.EndsWith(".png")) ? "image/png" : "text/html";
        //}
    }
}
