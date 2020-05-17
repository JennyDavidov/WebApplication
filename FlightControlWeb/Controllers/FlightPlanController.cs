using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        public IDictionary<string, FlightPlan> DictionaryPlan = new Dictionary<string, FlightPlan>();
        private IPlanManager PlanModel = new MyPlanManager();

        // GET: api/FlightPlan/{id}
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan Get(string id)
        {
            DictionaryPlan = PlanModel.GetDictionary();
            FlightPlan p = DictionaryPlan.FirstOrDefault(t => t.Key == id).Value;
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
