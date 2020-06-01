using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class FlightPlan
    {
        public int Passengers { get; set; }
        public string CompanyName { get; set; }
        public InitialLocation InitialLocation { get; set; }
        public List<Segment> Segments { get; set; }

        public FlightPlan(int Passengers1, string CompanyName1, InitialLocation Initial1,
           List<Segment> seg1)
        {
            Passengers = Passengers1;
            CompanyName = CompanyName1;
            InitialLocation = Initial1;
            Segments = seg1;
        }
        public FlightPlan()
        {
        }
    }
}