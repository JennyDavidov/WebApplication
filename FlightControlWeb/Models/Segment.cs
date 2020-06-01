namespace FlightControl.Models
{
    public class Segment
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
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