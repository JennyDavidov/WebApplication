using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControl.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net;

namespace FlightControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private IFlightsManager Model = new MyFlightManager();
        private IPlanManager PlanModel = new MyPlanManager();
        private IServersManager ServerModel = new MyServersManager();
        private HttpClient client;

        public FlightsController(IHttpClientFactory client1)
        {
            this.client = client1.CreateClient();
        }

        // GET: api/Flights/
        [HttpGet]
        //[HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<Flight[]>> Get(string relative_to)
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
                    FlightPlan p;
                    PlanModel.GetAllPlans().TryGetValue(f.Value.Flight_id, out p);
                    //find the start time as appear in the initial time in the flight plan
                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(p.Initial_Location.Date_time));
                    //update current location of flight
                    CurrentFlightLocation(startTime, parsedDate, f.Value);
                    DateTime endTime = CalcEndTime(startTime, f.Value);
                    //update end time of flight
                    f.Value.EndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
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
                //Sync all: Its the second GET request: ALL flights from this time
                //FIRST PART - get all internal flights (flights source: drag & drop)
                DateTime parsedDate = DateTime.Parse(relative_to);
                parsedDate = TimeZoneInfo.ConvertTimeToUtc(parsedDate);
                foreach (var f in Model.GetAllFlights())
                {
                    FlightPlan p;
                    PlanModel.GetAllPlans().TryGetValue(f.Value.Flight_id, out p);
                    //find the start time as appear in the initial time in the flight plan
                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(p.Initial_Location.Date_time));
                    //update current location of flight
                    CurrentFlightLocation(startTime, parsedDate,f.Value);
                    DateTime endTime = CalcEndTime(startTime, f.Value);
                    //update end time of flight
                    f.Value.EndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    int resultAfterStart = DateTime.Compare(parsedDate, startTime);
                    int resultBeforeEnd = DateTime.Compare(parsedDate, endTime);
                    if ((resultAfterStart >= 0) && (resultBeforeEnd <= 0))
                    {
                        returnList.Add(f.Value);
                    }
                }
                //second part - get all exnternal flights (flights source: other servers)
                foreach (var server in ServerModel.GetAllServers())
                {
                    try
                    {
                        string url = server.Value.ServerURL + "/api/Flights?relative_to=" + relative_to;
                        var contentt = await this.client.GetStringAsync(url);
                        List<Flight> flightsFromServer = JsonConvert.DeserializeObject<List<Flight>>(contentt);
                        foreach (var externalFlight in flightsFromServer)
                        {
                            ServerModel.GetServerToFlightDic().AddOrUpdate(externalFlight.Flight_id, server.Value,(oldKey,oldVal) => server.Value);
                            externalFlight.Is_external = true;

                            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(externalFlight.Date_time));
                            DateTime endTime = CalcEndTime(startTime, externalFlight);
                            int resultAfterStart = DateTime.Compare(parsedDate, startTime);
                            int resultBeforeEnd = DateTime.Compare(parsedDate, endTime);
                            if ((resultAfterStart >= 0) && (resultBeforeEnd <= 0))
                            {
                                returnList.Add(externalFlight);
                            }
                        }
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


        public DateTime CalcEndTime(DateTime startTime, Flight f)
        {
            DateTime fEndTime;
            FlightPlan p;
            double totalSeg = 0;
            foreach (var plan in PlanModel.GetAllPlans())
            {
                //find the flight plan
                if (string.Compare(plan.Key, f.Flight_id, true) == 0)
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

        public void CurrentFlightLocation(DateTime startTime, DateTime relativeTo, Flight f)
        {
            FlightPlan p;
            PlanModel.GetAllPlans().TryGetValue(f.Flight_id, out p);
            int i;
            TimeSpan timeSpan = relativeTo - startTime;
            double diff = timeSpan.TotalSeconds;
            //if its in the first segment
            if(p.Segments[0].Timespan_seconds >= diff)
            {
                double percentage = (diff / p.Segments[0].Timespan_seconds);
                //calc: (end value-start value)* percentage
                f.Latitude = p.Initial_Location.Latitude + (p.Segments[0].Latitude- p.Initial_Location.Latitude) * percentage;
                f.Longitude = p.Initial_Location.Longitude + (p.Segments[0].Longitude - p.Initial_Location.Longitude) * percentage;
            }
            else
            {
                diff = diff - p.Segments[0].Timespan_seconds;
                for(i=1; i < p.Segments.Count; i++)
                {
                    if (p.Segments[i].Timespan_seconds >= diff)
                    {
                        double percentage = (diff / p.Segments[i].Timespan_seconds);
                        //calc: (end value-start value)* percentage
                        f.Latitude = p.Segments[i - 1].Latitude + (p.Segments[i].Latitude - p.Segments[i-1].Latitude) * percentage;
                        f.Longitude = p.Segments[i - 1].Longitude + (p.Segments[i].Longitude - p.Segments[i-1].Longitude) * percentage;
                        break;
                    }
                    else
                    {
                        diff -= p.Segments[i].Timespan_seconds;
                    }
                }
            } 
        }

    }
}
