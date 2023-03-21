using FSAutomator.Backend.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class CalculateDistanceToCoordinates : IAction
    {

        public double FinalLatitude { get; set; }
        public double FinalLongitude { get; set; }

        public double currentLatitude;

        public double currentLongitude;

        public CalculateDistanceToCoordinates()
        {

        }

        public CalculateDistanceToCoordinates(double lat, double lon)
        {
            this.FinalLatitude = lat;
            this.FinalLongitude = lon;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            if (!GetCurrentCoordinates(sender, connection))
            {
                return new ActionResult("An error ocurred while getting current coordinates", null, true);
            }

            Coordinate origin = new Coordinate()
            {
                Latitude = currentLatitude,
                Longitude = currentLongitude
            };

            Coordinate destination = new Coordinate()
            {
                Latitude = FinalLatitude,
                Longitude = FinalLongitude
            };

            double distance = GeoCalculator.GetDistance(origin, destination, 2, DistanceUnit.Kilometers);

            return new ActionResult($"{distance} Km.", distance.ToString(), false);
        }

        private bool GetCurrentCoordinates(object sender, SimConnect connection)
        {
            var currentLatitude = new GetVariable("PLANE LATITUDE").ExecuteAction(sender, connection);
            this.currentLatitude = Convert.ToDouble(currentLatitude.ComputedResult);

            var currentLongitude = new GetVariable("PLANE LONGITUDE").ExecuteAction(sender, connection);
            this.currentLongitude = Convert.ToDouble(currentLongitude.ComputedResult);

            return currentLatitude.Error && currentLongitude.Error;
        }
    }
}
