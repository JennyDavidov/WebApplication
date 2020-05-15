﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyFlightManagers : IFlightsManager
    {
        private static List<Flight> Flights = new List<Flight>()
        {
            new Flight {Flight_id= 11111,Longitude= 31.244,Latitude= 31.12, Passengers= 216,
                Company_name="SwissAir",Date_time= "2020-12-26T23:56:21Z",Is_external= false },

            new Flight {Flight_id= 22222,Longitude= 32.244,Latitude= 32.12, Passengers= 200,
                Company_name="El-Al",Date_time= "2020-12-26T23:56:21Z",Is_external= false },

            new Flight {Flight_id= 33333,Longitude= 33.244,Latitude= 33.12, Passengers= 100,
                Company_name="Lufthansa",Date_time= "2020-12-26T23:56:21Z",Is_external= false },

            new Flight {Flight_id= 44444,Longitude= 34.244,Latitude= 34.12, Passengers= 500,
                Company_name="Arkia",Date_time= "2020-12-26T23:56:21Z",Is_external= false },

            new Flight {Flight_id= 55555,Longitude= 35.244,Latitude= 35.12, Passengers= 156,
                Company_name="Cathay Pacific",Date_time= "2020-12-26T23:56:21Z",Is_external= false },

            new Flight {Flight_id= 66666,Longitude= 36.244,Latitude= 36.12, Passengers= 1000,
                Company_name="Thai airlines",Date_time= "2020-12-26T23:56:21Z",Is_external= false },

        };
        public void AddFlight(Flight f)
        {
            Flights.Add(f);
        }

        public void DeleteFlight(int flight_id)
        {
            Flight f = Flights.Where(x => x.Flight_id == flight_id).FirstOrDefault();
            if (f == null)
            {
                throw new Exception("Flight not found");
            }
            else
            {
                Flights.Remove(f);
            }
        }

        public IEnumerable<Flight> GetAllFlights()
        {
            return Flights;
        }

        public void UpdateFlight(Flight f)
        {
            Flight a = Flights.Where(x => x.Flight_id == f.Flight_id).FirstOrDefault();
            a.Company_name = f.Company_name;
            a.Date_time = f.Company_name;
            a.Latitude = f.Latitude;
            a.Longitude = f.Longitude;
            a.Passengers = f.Passengers;
            a.Is_external = f.Is_external;



        }
    }
}
