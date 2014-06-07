using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class ServerController : BaseController
    {
        public override void Init()
        {
            Console.WriteLine(Request.Url);
            switch (Request.Url)
            {
                case "/index.html":
                    if (Request.Method == "POST")
                        Index();
                    break;
            }
        }

        public void Index()
        {
            Response.StatusCode = 307;
            Response.StatusMessage = "Redirect";
            Response.AddHeader("Location","page.html");
        }
    }
}
