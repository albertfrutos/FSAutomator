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
            // comentari provar

            var origin = GetCurrentCoordinates(sender, connection);

            if (origin is null)
            {
                return new ActionResult("An error ocurred while getting current coordinates", null, true);
            }

            Coordinate destination = new Coordinate()
            {
                Latitude = FinalLatitude,
                Longitude = FinalLongitude
            };

            double distance = GeoCalculator.GetDistance((Coordinate)origin, destination, 2, DistanceUnit.Kilometers);

            return new ActionResult($"{distance} Km.", distance.ToString(), false);
        }

        private Coordinate? GetCurrentCoordinates(object sender, ISimConnectBridge connection)
        {
            try
            {
                var coordinates = new Coordinate();

                getVariable.VariableName = "PLANE LATITUDE";
                var currentLatitude = getVariable.ExecuteAction(sender, connection);
                coordinates.Latitude = Convert.ToDouble(currentLatitude.ComputedResult);

                getVariable.VariableName = "PLANE LONGITUDE";
                var currentLongitude = getVariable.ExecuteAction(sender, connection);
                coordinates.Longitude = Convert.ToDouble(currentLongitude.ComputedResult);

                if (currentLatitude.Error || currentLongitude.Error)
                {
                    return null;
                }

                return coordinates;
            }
            catch
            {
                return null;
            }
            

        }
    }
}
