using FSAutomator.Backend.Actions.Base;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using Geolocation;

namespace FSAutomator.Backend.Actions
{
    public class CalculateDistanceToCoordinates : ActionBase, IAction
    {

        public double FinalLatitude { get; set; }
        public double FinalLongitude { get; set; }

        public double currentLatitude;

        public double currentLongitude;

        IGetVariable getVariable;

        public CalculateDistanceToCoordinates()
        {

        }

        public CalculateDistanceToCoordinates(double lat, double lon, IGetVariable getVariable) : base(getVariable)
        {
            this.FinalLatitude = lat;
            this.FinalLongitude = lon;
            this.getVariable = getVariable;
        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
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

        private bool GetCurrentCoordinates(object sender, ISimConnectBridge connection)
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
