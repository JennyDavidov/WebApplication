using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class InitialLocation
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string DateTime { get; set; }

        public InitialLocation(double longitude1, double latitude1, string dateTime1)
        {
            Longitude = longitude1;
            Latitude = latitude1;
            DateTime = dateTime1;
        }
        public InitialLocation() { }

    }
}
