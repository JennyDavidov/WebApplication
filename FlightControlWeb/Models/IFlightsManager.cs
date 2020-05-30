using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace FlightControl.Models
{
    interface IFlightsManager
    {
        //IEnumerable is interface of collections
        ConcurrentDictionary<string, Flight> GetAllFlights();
        void AddFlight(Flight f);
        void DeleteFlight(string flight_id);
        void UpdateFlight(Flight f);
    }
}
