using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IFlightsManager
    {
        //IEnumerable is interface of collections
        IEnumerable<Flight> GetAllFlights();
        void AddFlight(Flight f);
        void DeleteFlight(int flight_id);
        void UpdateFlight(Flight f);
    }
}
