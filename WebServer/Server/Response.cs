using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class Response : WebObject
    {
        public string Path { get { return _path; } set { _path = value; PathExtension = _path.Substring(_path.LastIndexOf('.')+1, _path.Length - (_path.LastIndexOf('.')+1)).ToLower(); } }
        public string PathExtension { get; set; }
        public string Url { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get { return PathExtensionToContentType(); } }

        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        //public string HTTPVersion { get; set; }

        public string CurrentDate { get { return String.Format("{0:ddd dd MMM yy HH:mm:ss} GMT", DateTime.Now); } }

        //private Dictionary<string, string> _headers;
        private string _path;

        public Response() : base()
        {
            HTTPVersion = "HTTP/1.1";
        }

        public Response(Request request) : base()
        {
            HTTPVersion = (!String.IsNullOrEmpty(request.HTTPVersion)) ? request.HTTPVersion : "HTTP/1.1";
            Url = request.Url;
        }

        public bool IsErrorResponse()
        {
            return (StatusCode > 400 && StatusCode < 600);
        }

        public string GetStatusAsString()
        {
            return HTTPVersion + " " + StatusCode + " " + StatusMessage + Environment.NewLine;
        }

        public string GetHeadersAsString()
        {
            string headerStr = "";
            foreach(KeyValuePair<string, string> header in _headers) {
                headerStr += header.Key + ": " + header.Value + Environment.NewLine;
            }
            return headerStr + Environment.NewLine;
        }

        public string GetFullResponseHeadersAsString()
        {
            return GetStatusAsString() + CurrentDate + Environment.NewLine + GetHeadersAsString();
        }

        public byte[] GetResponseHeaderAsByteArray()
        {
            return Encoding.UTF8.GetBytes(GetFullResponseHeadersAsString());
        }

        private string PathExtensionToContentType()
        {
            //Console.WriteLine(PathExtension);
            switch (PathExtension)
            {
                case "png": case "jpg": case "jpeg":
                    return "image/"+PathExtension;
                case "js":
                    return "text/javascript";
                case "css":
                    return "text/css";
                case "txt":
                    return "text/plain";
                default:
                    return "text/html";
            }
        }

        // Set default headers true if success, false if response is invalid

        public bool SetDefaultHeaders()
        {
            bool valid = false;
            if (Content != null)
            {
                AddHeader("Accept-Ranges", "bytes");
                AddHeader("Connection", "keep-alive");
                AddHeader("Content-Length", Content.Length.ToString());
                AddHeader("Content-Type", ContentType);
                AddHeader("Cache-Control", "none");
                AddHeader("Date", CurrentDate);
                AddHeader("Keep-Alive", "timeout=5, max=100");
                AddHeader("Last-Modified", CurrentDate);
                valid = true;
            }
            else
            {
                StatusCode = 404;
                StatusMessage = "Not Found";
                AddHeader("Connection", "keep-alive");
                AddHeader("Content-type", "text/html");
                AddHeader("Cache-Control", "no-cache, must-revalidate");
                valid = false;
            }
            AddHeader("Server", ServerConfig.Name);

            return valid;
        }

    }
}
