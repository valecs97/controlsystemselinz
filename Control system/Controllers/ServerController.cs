using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Control_system.Controllers
{
    public class ServerController : ApiController
    {
        private routeController rc;
        private upload up;
        repositoryMap rm;
        public ServerController()
        {
            rm = new repositoryMap();
            rc = new routeController(rm);
            up = new upload();
        }
        ~ServerController()
        {
            rc = null;
            rm = null;
            up = null;
        }

        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            string data = request.Content.ReadAsStringAsync().Result;
            string message = up.decodeCommand(data, rc,rm);
            if (message=="fail")
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            if (message== "succeded")
                return Request.CreateResponse(HttpStatusCode.OK);
            return new HttpResponseMessage() { Content = new StringContent(message) };
        }
    }
}