using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    abstract class WebObject
    {
        public string HTTPVersion { get; set; }

        protected Dictionary<string, string> _headers;

        public WebObject()
        {
            _headers = new Dictionary<string, string>();
        }

        public void AddHeader(string header, string value)
        {
            _headers.Add(header, value);
        }

        public string GetHeader(string header)
        {
            if(_headers.ContainsKey(header))
                return _headers[header];
            return null;
        }

        public bool RemoveHeader(string header)
        {
            return _headers.Remove(header);
        }

        public void ClearHeaders()
        {
            _headers.Clear();
        }
    }
}
