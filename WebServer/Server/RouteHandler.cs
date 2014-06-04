using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Server
{
    class RouteHandler
    {
        private Request _request;
        public Response _response;

        public RouteHandler(Request request, Response response)
        {
            _request = request;
            _response = response;
        }

        public void RouteUrl()
        {
            Console.WriteLine(_request.Port);
            // Console.WriteLine("Route URL here");
            BaseController controller = null;
            if (_request.Port == ServerConfig.WebPort)
            {
                //RouteWeb();
                controller = new ServerController();
            }
            else if (_request.Port == ServerConfig.ControlPort)
            {
                //RouteControl();
                controller = new ControlServerController();
            }
            controller.Request = _request;
            controller.Response = _response;

            controller.Init();
        }

        //public void RouteWeb()
        //{
        //    new 
        //}

        //public void RouteControl()
        //{
        //    Console.WriteLine("Control Route");
        //}
    }
}
