namespace FSAutomator.Backend.Entities
{
    public class Logger
    {
        internal List<Point> Points = new List<Point>();

        public void AddPoint(string latitude, string longitude, string altitude)
        {
            this.Points.Add(new Point(latitude, longitude, altitude));
        }
    }
}