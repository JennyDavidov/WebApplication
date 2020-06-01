using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FlightControl.Models
{
    public class InitialLocation
    {
        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("date_time")]
        [JsonProperty("date_time")]
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
