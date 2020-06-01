using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControl.Models;
using System.Collections.Concurrent;

namespace FlightControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private IServersManager model = new MyServersManager();
        // GET: api/Servers
        [HttpGet]
        public ConcurrentDictionary<string, Servers> Get()
        {
            return model.GetAllServers();
        }


        // POST: api/Servers
        [HttpPost]
        public Servers Post(Servers s)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(s.ServerURL, UriKind.Absolute, out uriResult)
     && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result==false)
            {
                throw new InvalidOperationException();
            }
            model.AddServer(s);
            return s;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Servers p;
            if (model.GetAllServers().TryGetValue(id, out p))
            {
                model.GetAllServers().TryRemove(id, out p);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
