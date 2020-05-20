using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IPlanManager
    {
        //IEnumerable is interface of collections
        ConcurrentDictionary<string, FlightPlan> GetAllPlans();
        void AddPlan(FlightPlan f);
        void DeletePlan(FlightPlan p);
    }
}
