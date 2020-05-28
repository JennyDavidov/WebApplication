using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlightControlWeb.Models;


namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private IServersManager Model = new MyServersManager();
        // GET: api/Servers
        [HttpGet]
        public IEnumerable<Servers> Get()
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
            foreach (Servers s in Model.GetAllServers())
            {
                //search server by id
                Console.WriteLine("hi");
                if (String.Equals(s.ServerId, id))
                {
                    Model.DeleteServer(s);
                    break;
                }
            }
        }
    }
}
