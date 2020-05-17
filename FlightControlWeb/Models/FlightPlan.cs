using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public struct Segment
    {
        public double Longitude;
        public double Latitude;
        public int timespan_seconds;
    }
    public struct Initial_location
    {
        public double Longitude;
        public double Latitude;
        public string Date_time;
    }
    public class FlightPlan
    {
        public int Passengers { get; set; }
        public string Company_name { get; set; }
        public Initial_location Initial { get; set; }
        public Segment seg { get; set; }
        public int Flight_id { get; set; }
        public bool Is_external { get; set; }

        public FlightPlan(int Passengers1, string Company_name1, Initial_location Initial1,
           Segment seg1, int Flight_id1, bool Is_external1)
        {
            Passengers = Passengers1;
            Company_name = Company_name1;
            Initial = Initial1;
            seg = seg1;
            Flight_id = Flight_id1;
            Is_external = Is_external1;
        }
    }
}