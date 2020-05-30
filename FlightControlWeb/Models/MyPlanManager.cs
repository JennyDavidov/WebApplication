using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class MyPlanManager : IPlanManager
    {
        IFlightsManager ModelFlight = new MyFlightManager();
        public static ConcurrentDictionary<string, FlightPlan> CachePlans = new ConcurrentDictionary<string, FlightPlan>();

        public void AddPlan(FlightPlan p)
        {
            //generate ID:
            string id = GenerateId();
            Flight flight = new Flight(id, p.Initial_Location.Longitude, p.Initial_Location.Latitude,
                p.Passengers, p.Company_name, p.Initial_Location.Date_time,false);
            ModelFlight.AddFlight(flight);
            if(!CachePlans.TryAdd(id, p))
            {
                Console.WriteLine("Error adding item to cache");
            }
            Console.WriteLine("add plan succeeded");
        }

        public void DeletePlan(FlightPlan p)
        {
            string key = CachePlans.FirstOrDefault(x => x.Value == p).Key;
            if (key == null)
            {
                throw new Exception("Flight Plan not found");
            }
            else
            {
                if (!CachePlans.TryRemove(key, out p))
                {
                    Console.WriteLine("Error removing item to cache");
                }
            }
        }

        public ConcurrentDictionary<string, FlightPlan> GetAllPlans()
        {
            return CachePlans;
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
