using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControl.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightPlanController : ControllerBase
    {
        private IPlanManager PlanModel = new MyPlanManager();

        // GET: api/FlightPlan/{id}
        [HttpGet("{id}", Name = "Get")]
        public FlightPlan Get(string id)
        {
            FlightPlan p = PlanModel.GetAllPlans().FirstOrDefault(t => string.Compare(t.Key,id,true)==0).Value;
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
