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
        }
    }
}
