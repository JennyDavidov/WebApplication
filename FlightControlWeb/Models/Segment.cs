using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace FlightControl.Models
{
    public class Segment
    {
        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("timespan_seconds")]
        [JsonProperty("timespan_seconds")]
        public double TimespanSeconds { get; set; }

        public Segment(double longitude, double latitude, int timespanSeconds)
        {
            Longitude = longitude;
            Latitude = latitude;
            TimespanSeconds = timespanSeconds;
        }
        public Segment() { }
    }
}