using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IPlanManager
    {
        //IEnumerable is interface of collections
        IEnumerable<FlightPlan> GetAllPlans();
        void AddPlan(FlightPlan f);
        void DeletePlan(FlightPlan p);
        IDictionary<string, FlightPlan> GetDictionary();
    }
}
