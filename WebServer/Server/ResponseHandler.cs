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
            response.AddHeader("Connection", "keep-alive");
            response.AddHeader("Content-type", "text/html");
            response.AddHeader("Cache-Control", "no-cache, must-revalidate");
            response.AddHeader("Server", ServerConfig.Name);
            response.Content = GetErrorPage(response.StatusCode);

            ReturnResponse(client.GetStream(), response);
        }

        public void HandlePost(Request request, Response response)
        {
            try
            {
                if (request.Values.Count > 0)
                {
                    String webPort = request.Values["web-port"];
                    String controlPort = request.Values["control-port"];
                    String webRoot = request.Values["webroot"];
                    String defaultPage = request.Values["default-page"];
                    // Write config file
                    ServerConfig.WriteConfig(webPort, controlPort, webRoot, defaultPage);
                    // Set response
                    response.Content = Encoding.ASCII.GetBytes("Config saved!");
                }
            }
            catch (KeyNotFoundException) { }
        }

        public void HandleResponse(TcpClient client, Request request, String root)
        {
            NetworkStream stream = client.GetStream();
            //@"C:\Users\Remi\Documents\GitHub\WebServer\WebServer\index.html"
            Response response = GetResponseForRequest(request, root);
            
            Console.WriteLine(response.GetHeadersAsString());

            ReturnResponse(stream, response);
        }

        public Response GetResponseForRequest(Request request, string root)
        {
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
                // Parse config
                //HTMLParser parser = new HTMLParser();
                //string content = parser.ParseHTMLConfig(response.Path);
                //response.Content = Encoding.ASCII.GetBytes(content);
            }

            if(!response.SetDefaultHeaders())
                response.Content = GetErrorPage(response.StatusCode);

            return response;
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
            //string[] urlParts = url.Split('/');
            //if (String.IsNullOrEmpty(urlParts[urlParts.Length - 1]))
            if(!url.EndsWith("/"))
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
