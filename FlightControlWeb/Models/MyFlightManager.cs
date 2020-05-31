using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class MyFlightManager : IFlightsManager
    {
        private static int index = 0;
        public static ConcurrentDictionary<string, Flight> CacheFlights = new ConcurrentDictionary<string, Flight>();

        public void AddFlight(Flight f)
        {
            if (!CacheFlights.TryAdd(f.Flight_id, f))
            {
                Console.WriteLine("Error adding item to cache");
            }
            else
            {
                index++;
            }
        }

        public void DeleteFlight(string flight_id)
        {
            string key = null;
            Flight f;
            foreach (var item in CacheFlights)
            {
                if (string.Compare(item.Value.Flight_id, flight_id, true) == 0)
                {
                    key = item.Key;
                    f = item.Value;
                    break;
                }
            }
            if (key == null)
            {
                throw new Exception("Flight not found");
            }
            else
            {
                CacheFlights.TryRemove(key, out f);
            }
        }

        public ConcurrentDictionary<string, Flight> GetAllFlights()
        {
            return CacheFlights;
        }

        public void UpdateFlight(Flight f)
        {
            Flight a = null;
            foreach (var item in CacheFlights)
            {
                if (string.Compare(item.Value.Flight_id, f.Flight_id, true) == 0)
                {
                    a = item.Value;
                    break;
                }
            }
            if (a == null)
            {
                throw new Exception("Flight not found");
            }
            else
            {
                a.Company_name = f.Company_name;
                a.Date_time = f.Company_name;
                a.Latitude = f.Latitude;
                a.Longitude = f.Longitude;
                a.Passengers = f.Passengers;
                a.Is_external = f.Is_external;
            }
        }
    }
}
