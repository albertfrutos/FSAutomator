namespace FSAutomator.Backend.Entities
{
    public class Point
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Altitude { get; set; }

        public Point(string latitude, string longitude, string altitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.Altitude = altitude;
        }

        public Point()
        {

        }
    }


}
