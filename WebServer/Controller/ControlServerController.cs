using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class ControlServerController : BaseController
    {
        public override void Init()
        {
            Console.WriteLine(Request.Url);
            switch (Request.Url)
            {
                case "insert url here":
                    // Call function for URL ...
                    break;
            }
        }
    }
}
