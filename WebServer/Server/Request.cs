using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class Request : WebObject
    {
        public string Method { get; set; }
        public string FullUrl { get; set; }
        public string Url { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public string Body { get; set; }
        public string Id { get; set; }

        public string IPAdress { get; set; }

        public Request() : base()
        {
            Values = new Dictionary<string, string>();
        }

        public String GetValue(String key)
        {
            if (Values.ContainsKey(key))
            {
                return Values[key];
            }
            return null;
        }
    }
}
