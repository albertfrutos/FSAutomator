using FSAutomator.Backend.Actions.Base;
using FSAutomator.Backend.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class CalculateBearingToCoordinates : ActionBase, IAction
    {

        public double FinalLatitude { get; set; }
        public double FinalLongitude { get; set; }

        public double currentLatitude;

        public double currentLongitude;

        public CalculateBearingToCoordinates(double lat, double lon, IGetVariable getVariable):base(getVariable)
        {
            this.FinalLatitude = lat;
            this.FinalLongitude = lon;
            this.getVariable = getVariable;

        }

        public CalculateBearingToCoordinates()
        {

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

            double heading = GeoCalculator.GetBearing(origin, destination);

            return new ActionResult($"Heading to destination: {heading}", heading.ToString(), false);
        }

        private bool GetCurrentCoordinates(object sender, SimConnect connection)
        {
            getVariable.VariableName = "PLANE LATITUDE";
            var currentLatitude = getVariable.ExecuteAction(sender, connection);
            this.currentLatitude = Convert.ToDouble(currentLatitude.ComputedResult);

            getVariable.VariableName = "PLANE LONGITUDE";
            var currentLongitude = getVariable.ExecuteAction(sender, connection);
            this.currentLongitude = Convert.ToDouble(currentLongitude.ComputedResult);

            return !(currentLatitude.Error || currentLongitude.Error);
        }
    }
}
