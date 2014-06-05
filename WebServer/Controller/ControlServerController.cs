using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.Utilities.HTML;

namespace WebServer.Server
{
    class ControlServerController : BaseController
    {
        public override void Init()
        {
            Console.WriteLine(Request.Url);
            switch (Request.Url)
            {
                case "/configuration.html":
                    if (Request.Method == "GET")
                        GetConfiguration();
                    else if (Request.Method == "POST")
                        PostConfiguration();
                    break;
            }
        }

        private void GetConfiguration()
        {
            HTMLParser parser = new HTMLParser();
            string content = parser.ParseHTMLConfig(Response.Path);
            Response.Content = Encoding.UTF8.GetBytes(content);
        }

        private void PostConfiguration()
        {
            try
            {
                if (Request.Values.Count > 0)
                {
                    String webPort = Request.Values["web-port"];
                    String controlPort = Request.Values["control-port"];
                    String webRoot = Request.Values["webroot"];
                    String defaultPage = Request.Values["default-page"];
                    // Write config file
                    ServerConfig.WriteConfig(webPort, controlPort, webRoot, defaultPage);
                    // Restart server
                    Server.RestartId = Request.Id;
                    // Set response
                    GetConfiguration();
                }
            }
            catch (KeyNotFoundException) { }
        }

    }
}
