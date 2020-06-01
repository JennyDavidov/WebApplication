using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class Flight
    {
        public string FlightId { get; set; }

        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }

        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("date_time")]
        [JsonProperty("date_time")]
        public string DateTime { get; set; }

        [JsonProperty("is_external")]
        [JsonPropertyName("is_external")]
        public bool IsExternal { get; set; }

        public string EndTime { get; set; }



        public Flight(string flightId1, double longitude1, double latitude1,
            int passengers1, string companyName1, string dateTime1, bool isExternal1)
        {
            FlightId = flightId1;
            Longitude = longitude1;
            Latitude = latitude1;
            Passengers = passengers1;
            CompanyName = companyName1;
            DateTime = dateTime1;
            IsExternal = isExternal1;
        }
    }
}
