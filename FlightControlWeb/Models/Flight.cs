using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        public string Flight_id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Passengers { get; set; }
        public string Company_name { get; set; }
        public string Date_time { get; set; }
        public bool Is_external { get; set; }

        public Flight(string Flight_id1, double Longitude1, double Latitude1,
            int Passengers1, string Company_name1, string Date_time1, bool Is_external1)
        {
            Flight_id = Flight_id1;
            Longitude = Longitude1;
            Latitude = Latitude1;
            Passengers = Passengers1;
            Company_name = Company_name1;
            Date_time = Date_time1;
            Is_external = Is_external1;
        }
    }
}
