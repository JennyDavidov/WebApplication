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
        private IFlightsManager model = new MyFlightManager();
        private IPlanManager planModel = new MyPlanManager();
        private IServersManager serverModel = new MyServersManager();
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
            bool isSync = false;
            string s = Request.QueryString.Value;
            if (s.Contains("sync_all"))
            {
                isSync = true;
            }
            List<Flight> returnList = new List<Flight>();
            //if its the first GET request:
            // return array of that: 1.externalIs=false 2. Time is now
            if (!isSync)
            {
                DateTime parsedDate = DateTime.Parse(relative_to);
                parsedDate = TimeZoneInfo.ConvertTimeToUtc(parsedDate);
                foreach (var f in model.GetAllFlights())
                {
                    FlightPlan p;
                    planModel.GetAllPlans().TryGetValue(f.Value.FlightId, out p);
                    //find the start time as appear in the initial time in the flight plan

                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(p.InitialLocation.DateTime));
                    //update current location of flight
                    CurrentFlightLocation(startTime, parsedDate, f.Value);
                    DateTime endTime = CalcEndTime(startTime, f.Value);
                    //update end time of flight
                    f.Value.EndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    int resultAfterStart = DateTime.Compare(parsedDate, startTime);
                    int resultBeforeEnd = DateTime.Compare(parsedDate, endTime);
                    if ((resultAfterStart >= 0) && (resultBeforeEnd <= 0))
                    {
                        if (f.Value.IsExternal == false)
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
                foreach (var f in model.GetAllFlights())
                {
                    FlightPlan p;

                    planModel.GetAllPlans().TryGetValue(f.Value.FlightId, out p);
                    //find the start time as appear in the initial time in the flight plan
                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(p.InitialLocation.DateTime));
                    //update current location of flight
                    CurrentFlightLocation(startTime, parsedDate, f.Value);
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
                bool flagIsError = false;
                foreach (var server in serverModel.GetAllServers())
                {
                    try
                    {
                        string url = server.Value.ServerURL + "/api/Flights?relative_to=" + relative_to;
                        List<Flight> flightsFromServer;
                        try
                        {
                            var contentt = await this.client.GetStringAsync(url);
                            flightsFromServer = JsonConvert.DeserializeObject<List<Flight>>(contentt);
                        }
                        catch
                        {
                            flagIsError = true;
                            continue;
                        }

                        foreach (var externalFlight in flightsFromServer)
                        {
                            serverModel.GetServerToFlightDic().AddOrUpdate(externalFlight.FlightId, server.Value, (oldKey, oldVal) => server.Value);
                            externalFlight.IsExternal = true;
                            string url2 = server.Value.ServerURL + "/api/FlightPlan/" + externalFlight.FlightId;
                            var plan = await this.client.GetStringAsync(url2);
                            FlightPlan planFromServer = JsonConvert.DeserializeObject<FlightPlan>(plan);

                            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(planFromServer.InitialLocation.DateTime));

                            DateTime endTime = CalcEndTimeForExternal(startTime, planFromServer);
                            externalFlight.EndTime = endTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                            int resultAfterStart = DateTime.Compare(parsedDate, startTime);
                            int resultBeforeEnd = DateTime.Compare(parsedDate, endTime);
                            if ((resultAfterStart >= 0) && (resultBeforeEnd <= 0))
                            {
                                returnList.Add(externalFlight);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;

                    }
                }
                if (flagIsError)
                {
                    return BadRequest();
                }
                Flight[] array = returnList.ToArray();
                return array;
            }
        }



        // POST: api/Flights
        [HttpPost]
        public Flight AddFlight(Flight f)
        {
            model.AddFlight(f);
            return f;
        }

        // DELETE: api/Flights/id
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            FlightPlan p;
            if (planModel.GetAllPlans().TryGetValue(id, out p))
            {
                planModel.GetAllPlans().TryRemove(id, out p);
                model.DeleteFlight(id);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }


        public DateTime CalcEndTime(DateTime startTime, Flight f)
        {
            DateTime fEndTime;
            FlightPlan p;
            double totalSeg = 0;
            foreach (var plan in planModel.GetAllPlans())
            {
                //find the flight plan
                if (string.Compare(plan.Key, f.FlightId, true) == 0)
                {
                    p = plan.Value;
                    foreach (var seg in p.Segments)
                    {
                        totalSeg = totalSeg + seg.TimespanSeconds;
                    }

                }
            }
            fEndTime = startTime.AddSeconds(totalSeg);
            return fEndTime;
        }

        public DateTime CalcEndTimeForExternal(DateTime startTime, FlightPlan p)
        {
            DateTime fEndTime;
            double totalSeg = 0;



            foreach (var seg in p.Segments)
            {
                totalSeg = totalSeg + seg.TimespanSeconds;
            }

            fEndTime = startTime.AddSeconds(totalSeg);
            return fEndTime;
        }

        public void CurrentFlightLocation(DateTime startTime, DateTime relativeTo, Flight f)
        {
            FlightPlan p;
            planModel.GetAllPlans().TryGetValue(f.FlightId, out p);
            int i;
            TimeSpan timeSpan = relativeTo - startTime;
            double diff = timeSpan.TotalSeconds;
            //if its in the first segment
            if (p.Segments[0].TimespanSeconds >= diff)
            {
                double percentage = (diff / p.Segments[0].TimespanSeconds);
                //calc: (end value-start value)* percentage
                f.Latitude = p.InitialLocation.Latitude + (p.Segments[0].Latitude - p.InitialLocation.Latitude) * percentage;
                f.Longitude = p.InitialLocation.Longitude + (p.Segments[0].Longitude - p.InitialLocation.Longitude) * percentage;
            }
            else
            {
                diff = diff - p.Segments[0].TimespanSeconds;
                for (i = 1; i < p.Segments.Count; i++)
                {
                    if (p.Segments[i].TimespanSeconds >= diff)
                    {
                        double percentage = (diff / p.Segments[i].TimespanSeconds);
                        //calc: (end value-start value)* percentage
                        f.Latitude = p.Segments[i - 1].Latitude + (p.Segments[i].Latitude - p.Segments[i - 1].Latitude) * percentage;
                        f.Longitude = p.Segments[i - 1].Longitude + (p.Segments[i].Longitude - p.Segments[i - 1].Longitude) * percentage;
                        break;
                    }
                    else
                    {
                        diff -= p.Segments[i].TimespanSeconds;
                    }
                }
            }
        }

    }
}
