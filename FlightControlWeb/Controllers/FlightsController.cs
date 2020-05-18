using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private IFlightsManager Model = new MyFlightManager();
        private IPlanManager PlanModel = new MyPlanManager();
        //// GET: api/Flights
        //[HttpGet]
        //public List<Flight> GetAllFlights()
        //{
        //    return Model.GetAllFlights();
        //}

        // GET: api/Flights/
        [HttpGet]
        //[HttpGet("{id}", Name = "Get")]
        public Flight[] Get(string relative_to)
        {
            bool sync_all = false;
            string s = Request.QueryString.Value;
            if (s.Contains("sync_all"))
            {
                sync_all = true;
            }
                List<Flight> returnList = new List<Flight>();
            //if its the first GET request:
            // return array of that: 1.external_is=false 2. Time is now
            if (!sync_all)
            {
                DateTime parsedDate = DateTime.Parse(relative_to);
                List<Flight> list = new List<Flight>();
                list = Model.GetAllFlights();
                foreach (Flight f in list)
                {
                    DateTime fDate = DateTime.Parse(f.Date_time);
                    if (fDate == parsedDate)
                    {
                        if (f.Is_external == false)
                        {
                            returnList.Add(f);
                        }
                    }
                }
                Flight[] array = returnList.ToArray();
                return array;
            }
            else
            {
                //Its the second GET request: ALL flights from this time
                DateTime parsedDate = DateTime.Parse(relative_to);
                List<Flight> list = new List<Flight>();
                list = Model.GetAllFlights();
                foreach (Flight f in list)
                {
                    DateTime fDate = DateTime.Parse(f.Date_time);
                    if (fDate == parsedDate)
                    {
                        returnList.Add(f);
                    }
                }
                Flight[] array = returnList.ToArray();
                return array;
            }
        }
        // POST: api/Flights
        [HttpPost]
        public Flight AddFlight(Flight f)
        {
            Model.AddFlight(f);
            return f;
        }

        // DELETE: api/Flights
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            FlightPlan p;
            if (PlanModel.GetDictionary().TryGetValue(id, out p))
            {
                PlanModel.GetDictionary().Remove(id);
            }
        }
    }
}
