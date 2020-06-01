using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace FlightControl.Models
{
    public class FlightPlan
    {
        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("initial_location")]
        [JsonProperty("initial_location")]
        public InitialLocation InitialLocation { get; set; }

        [JsonPropertyName("segments")]
        [JsonProperty("segments")]
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