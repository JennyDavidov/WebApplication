using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections;

namespace FlightControlWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private IFlightsManager Model = new MyFlightManager();
        private IPlanManager PlanModel = new MyPlanManager();

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
                parsedDate = TimeZoneInfo.ConvertTimeToUtc(parsedDate);
                foreach (var f in Model.GetAllFlights())
                {
                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(f.Value.Date_time));
                    DateTime endTime = CalcEndTime(startTime, f);
                    int resultAfterStart = DateTime.Compare(parsedDate, startTime);
                    int resultBeforeEnd = DateTime.Compare(parsedDate, endTime);
                    if ((resultAfterStart>=0) && (resultBeforeEnd <=0))
                    {
                        if (f.Value.Is_external == false)
                        {
                            returnList.Add(f.Value);
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
                parsedDate = TimeZoneInfo.ConvertTimeToUtc(parsedDate);
                foreach (var f in Model.GetAllFlights())
                {
                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(f.Value.Date_time));
                    DateTime endTime = CalcEndTime(startTime, f);
                    int resultAfterStart = DateTime.Compare(parsedDate, startTime);
                    int resultBeforeEnd = DateTime.Compare(parsedDate, endTime);
                    if ((resultAfterStart >= 0) && (resultBeforeEnd <= 0))
                    {
                        returnList.Add(f.Value);
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

        // DELETE: api/Flights/id
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            FlightPlan p;
            if (PlanModel.GetAllPlans().TryGetValue(id, out p))
            {
                PlanModel.GetAllPlans().TryRemove(id, out p);
                Model.DeleteFlight(id);
            }
        }
        public DateTime CalcEndTime(DateTime startTime, KeyValuePair<string, Flight> f)
        {
            DateTime fEndTime;
            FlightPlan p;
            double totalSeg = 0;
            foreach (var plan in PlanModel.GetAllPlans())
            {
                //find the flight plan
                if (string.Compare(plan.Key, f.Value.Flight_id, true) == 0)
                {
                    p = plan.Value;
                    foreach (var seg in p.Segments)
                    {
                        totalSeg = totalSeg + seg.Timespan_seconds;
                    }

                }
            }
            fEndTime = startTime.AddSeconds(totalSeg);
            return fEndTime;
        }
    }
}
