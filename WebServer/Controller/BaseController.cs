using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class BaseController
    {
        public Response Response { get; set; }
        public Request Request { get; set; }

        public virtual void Init()
        {
        }
    }
}
