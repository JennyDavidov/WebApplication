using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FlightControl.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private IPlanManager PlanModel = new MyPlanManager();
        private IServersManager ServerModel = new MyServersManager();
        private HttpClient client;


        public FlightPlanController(IHttpClientFactory client1)
        {
            this.client = client1.CreateClient();
        }

        // GET: api/FlightPlan/{id}
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<FlightPlan>> Get(string id)
        {
            FlightPlan p;
            if (PlanModel.GetAllPlans().FirstOrDefault(t => string.Compare(t.Key, id, true) == 0).Value != null)
            {
                p = PlanModel.GetAllPlans().FirstOrDefault(t => string.Compare(t.Key, id, true) == 0).Value;
            }
            else
            {
                try
                {
                    //search this id in external servers
                    Servers s;
                    ServerModel.GetServerToFlightDic().TryGetValue(id, out s);
                    string url = s.ServerURL + "/api/FlightPlan/" + id;
                    var contentt = await this.client.GetStringAsync(url);
                    p = JsonConvert.DeserializeObject<FlightPlan>(contentt);
                }
                catch (WebException e)
                {
                    return BadRequest();
                }
                catch (Exception e)
                {
                    return StatusCode(500);

                }
            }
            return p;
        }
        // POST: api/FlightPlan
        [HttpPost]
        public FlightPlan Post(FlightPlan p)
        {
            PlanModel.AddPlan(p);
            return p;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
