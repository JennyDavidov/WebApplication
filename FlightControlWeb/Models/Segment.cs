namespace FlightControlWeb.Models
{
    public class Segment
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Timespan_seconds { get; set; }

        public Segment(double longitude, double latitude, int timespan_seconds)
        {
            Longitude = longitude;
            Latitude = latitude;
            Timespan_seconds = timespan_seconds;
        }
        public Segment() { }
    }
}