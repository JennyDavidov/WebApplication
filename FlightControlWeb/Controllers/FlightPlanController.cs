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
        private IPlanManager planModel = new MyPlanManager();
        private IServersManager serverModel = new MyServersManager();
        private HttpClient client;


        public FlightPlanController(IHttpClientFactory client1)
        {
            this.client = client1.CreateClient();
        }

        // GET: api/FlightPlan/{id}
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<FlightPlan>> Get(string id)
        {
            FlightPlan p = null;
            if (planModel.GetAllPlans().FirstOrDefault(t => string.Compare(t.Key, id, true) == 0).Value != null)
            {
                p = planModel.GetAllPlans().FirstOrDefault(t => string.Compare(t.Key, id, true) == 0).Value;
            }
            else
            {
                try
                {
                    //search this id in external servers
                    Servers s;
                    serverModel.GetServerToFlightDic().TryGetValue(id, out s);
                    string url = s.ServerURL + "/api/FlightPlan/" + id;
                    var contentt = await this.client.GetStringAsync(url);
                    p = JsonConvert.DeserializeObject<FlightPlan>(contentt);
                }
                catch (WebException)
                {
                    return BadRequest();
                }
                catch (Exception)
                {
                    return StatusCode(500);

                }
            }
            if (p == null)
            {
                return StatusCode(400);
            }
            return p;
        }
        // POST: api/FlightPlan
        [HttpPost]
        public FlightPlan Post(FlightPlan p)
        {
            //check FP json validation validation
            bool segmentsIsValid = false;
            bool initialLocationIsValid = false;
            if ((p.InitialLocation != null))
            {
                initialLocationIsValid  = (p.InitialLocation.DateTime == null) || (p.InitialLocation.Latitude > 90)
                 || (p.InitialLocation.Latitude < -90) || (p.InitialLocation.Longitude > 180)
                 || (p.InitialLocation.Longitude < -180);
            }
            if ((p.Segments != null))
            {
                foreach(var  seg in p.Segments)
                {
                    bool result = (seg.Latitude < -90) || (seg.Latitude > 180) ||
                        (seg.Latitude < -90) || (seg.Longitude > 180) || (seg.TimespanSeconds <= 0);
                    segmentsIsValid = segmentsIsValid || result;
                }
            }

            bool isNull = (p.CompanyName == null) || (p.Passengers <= 0) || (p.Segments == null) || (p.InitialLocation == null)
                 || initialLocationIsValid || segmentsIsValid;

            if (isNull == true)
            {
                throw new InvalidOperationException();
            }

            planModel.AddPlan(p);
            return p;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
