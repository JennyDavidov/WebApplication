using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyPlanManager : IPlanManager
    {
        public static IDictionary<string, FlightPlan> IdToFPlan = new Dictionary<string, FlightPlan>();
        IFlightsManager ModelFlight = new MyFlightManager();
        private static List<FlightPlan> Plans = new List<FlightPlan>();
        //{
        //    new FlightPlan(126,"ElAl",new Initial_location(11.11,22.22,"2020-12-26T23:56:21Z"),new List<Segment> {
        //        new Segment(33.33,44.44,650)}),
        //     new FlightPlan(136,"ElAl2",new Initial_location(111.11,222.22,"2020-12-28T23:56:21Z"),new List<Segment> {
        //        new Segment(39.33,49.44,550)})
        //};

        public void AddPlan(FlightPlan p)
        {
            //generate ID:
            string id = GenerateId();
            Flight flight = new Flight(id, p.Initial_Location.Longitude, p.Initial_Location.Latitude,
                p.Passenger, p.Company_name, p.Initial_Location.Date_time,false);
            ModelFlight.AddFlight(flight);
            IdToFPlan.Add(id, p);
            Plans.Add(p);
        }

        public void DeletePlan(FlightPlan p)
        {
            string foundKey = IdToFPlan.FirstOrDefault(x => x.Value == p).Key;
            if (foundKey == null)
            {
                throw new Exception("Flight Plan not found");
            }
            else
            {
                IdToFPlan.Remove(foundKey);
            }
        }

        public IEnumerable<FlightPlan> GetAllPlans()
        {
            return Plans;
        }
        public IDictionary<string, FlightPlan> GetDictionary()
        {
            return IdToFPlan;
        }

        public string GenerateId()
        {
            Random rnd = new Random();
            StringBuilder builder = new StringBuilder();
            builder.Append(rnd.Next(1000, 9999).ToString());
            int num = rnd.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            builder.Append(let);
            int num2 = rnd.Next(0, 26); // Zero to 25
            char let2 = (char)('a' + num2);
            builder.Append(let2);
            builder.Append(rnd.Next(1000, 9999).ToString());
            return builder.ToString();
        }
    }
}
