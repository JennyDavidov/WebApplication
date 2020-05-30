using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class Initial_location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Date_time { get; set; }

        public Initial_location(double longitude1, double latitude1, string date_time1)
        {
            Longitude = longitude1;
            Latitude = latitude1;
            Date_time = date_time1;
        }
        public Initial_location() { }

    }
}
