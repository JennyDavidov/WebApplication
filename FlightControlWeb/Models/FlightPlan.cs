using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public int Passenger { get; set; }
        public string Company_name { get; set; }
        public Initial_location Initial_Location { get; set; }
        public List<Segment> Segments { get; set; }
        //public int Flight_id { get; set; }
        //public bool Is_external { get; set; }

        public FlightPlan(int Passengers1, string Company_name1, Initial_location Initial1,
           List<Segment> seg1)
        {
            Passenger = Passengers1;
            Company_name = Company_name1;
            Initial_Location = Initial1;
            Segments = seg1;
        }
        public FlightPlan()
        {
        }
    }
}