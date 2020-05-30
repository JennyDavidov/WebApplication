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
        private IServersManager Model = new MyServersManager();
        // GET: api/Servers
        [HttpGet]
        public ConcurrentDictionary<string, Servers> Get()
        {
            return Model.GetAllServers();
        }


        // POST: api/Servers
        [HttpPost]
        public Servers Post(Servers s)
        {
            Model.AddServer(s);
            return s;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            Servers p;
            if (Model.GetAllServers().TryGetValue(id, out p))
            {
                Model.GetAllServers().TryRemove(id, out p);
            }
        }
    }
}
