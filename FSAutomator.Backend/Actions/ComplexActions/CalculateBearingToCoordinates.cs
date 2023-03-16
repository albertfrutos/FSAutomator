using FSAutomator.Backend.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class CalculateBearingToCoordinates : IAction
    {

        public string FinalLatitude { get; set; }
        public string FinalLongitude { get; set; }

        public string currentLatitude = String.Empty;

        public string currentLongitude = String.Empty;


        public CalculateBearingToCoordinates(string lat, string lon)
        {
            this.FinalLatitude = lat;
            this.FinalLongitude = lon;
        }

        public CalculateBearingToCoordinates()
        {

        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            if (!Double.TryParse(this.FinalLatitude, out _) || !Double.TryParse(this.FinalLongitude, out _))
            {
                return new ActionResult("Coordinates are not a number", null, true);
            }

            if (GetCurrentCoordinates(sender, connection))
            {
                return new ActionResult("An error ocurred while getting current coordinates", null, true);
            }
            Coordinate origin = new Coordinate()
            {
                Latitude = Convert.ToDouble(currentLatitude),
                Longitude = Convert.ToDouble(currentLongitude)
            };
            Coordinate destination = new Coordinate()
            {
                Latitude = Convert.ToDouble(FinalLatitude),
                Longitude = Convert.ToDouble(FinalLongitude)
            };
            double heading = GeoCalculator.GetBearing(origin, destination);

            return new ActionResult($"Heading to destination: {heading}", heading.ToString(), false);
        }

        private bool GetCurrentCoordinates(object sender, SimConnect connection)
        {
            var currentLatitude = new GetVariable("PLANE LATITUDE").ExecuteAction(sender, connection);
            this.currentLatitude = currentLatitude.ComputedResult;

            var currentLongitude = new GetVariable("PLANE LONGITUDE").ExecuteAction(sender, connection);
            this.currentLongitude = currentLongitude.ComputedResult;

            return currentLatitude.Error || currentLongitude.Error;
        }
    }
}
