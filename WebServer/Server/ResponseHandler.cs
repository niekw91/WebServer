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

        public Response GetResponseForRequest(Request request, string root)
        {
            Response response = new Response(request);

            response.StatusCode = 200;
            response.StatusMessage = "OK";
            bool dirBrowsing = false;

            if (!ParseValidUrl(request.Url))
            {
                request.Url = FixUrl(request.Url);
                string defPage = GetDefaultPage(request, root);
                dirBrowsing = (ServerConfig.DirectoryBrowsing && defPage == null && request.Port == ServerConfig.WebPort);
                if (!dirBrowsing)
                    request.Url = request.Url + defPage;
                Console.WriteLine("Dir browsing: " + dirBrowsing);
            }

            response.Path = root + request.Url;
            if (File.Exists(response.Path) || dirBrowsing)
            {
                if (!dirBrowsing)
                    response.Content = File.ReadAllBytes(response.Path);
                else
                    response.Content = LoadDirBrowsingPage(response.Path);
            }

            if (!response.SetDefaultHeaders())
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
            String path = ServerConfig.Webroot + @"\errors\{0}.html";
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
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return url;
        }

        private string GetDefaultPage(Request request, string root)
        {
            string[] defaultPages = ServerConfig.DefaultPage.Split(';');
            foreach (string defPage in defaultPages)
            {
                if (File.Exists(root + request.Url + defPage))
                    return defPage;
            }
            return null;
        }

        private byte[] LoadDirBrowsingPage(string originPath)
        {
            string root = ServerConfig.Controlroot;
            string path = root + ServerConfig.DIR_BROWSING_FILE_PATH;

            if(Directory.Exists(originPath)) {
                DirectoryInfo directory = new DirectoryInfo(originPath);
                string[] files = directory.GetFiles().Select(f => f.Name).ToArray();
                string[] directories = directory.GetDirectories().Select(f => f.Name).ToArray();

                if(files.Length > 0 || directories.Length > 0) {
                    Utilities.HTML.HTMLParser parser = new Utilities.HTML.HTMLParser();
                    string dirPageContent = parser.ParseDirectoryBrowsingHTML(path, directories, files);

                    return Encoding.UTF8.GetBytes(dirPageContent);
                }
            }
            return null;
        }
    }
}
