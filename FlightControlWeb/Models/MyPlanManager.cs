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
        private static List<FlightPlan> Plans = new List<FlightPlan>()
        {
            new FlightPlan()
        }

        public void AddPlan(FlightPlan p)
        {
            //generate ID:
            string id = GenerateId();
            Flight flight = new Flight(id, p.Initial.Longitude, p.Initial.Latitude,
                p.Passengers, p.Company_name, p.Initial.Date_time, p.Is_external);
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
