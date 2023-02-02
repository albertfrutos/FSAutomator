using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class CalculateBearingToCoordinates : IAction
    {

        public double FinalLatitude { get; set; }
        public double FinalLongitude { get; set; }

        public string currentLatitude; 
        public string currentLongitude;



        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            GetCurrentCoordinates(sender, connection);
            Coordinate a = new Coordinate();
            Coordinate origin = new Coordinate()
            {
                Latitude = Convert.ToDouble(currentLatitude),
                Longitude = Convert.ToDouble(currentLongitude)
            };
            Coordinate destination = new Coordinate()
            {
                Latitude = FinalLatitude,
                Longitude = FinalLongitude
            };
            double heading = GeoCalculator.GetBearing(origin, destination);
            ReturnValueEvent.Invoke(this, heading.ToString());
            UnlockNextStep.Invoke(this, null);
        }

        private void GetCurrentCoordinates(object sender, SimConnect connection)
        {
            currentLatitude = new GetVariable("PLANE LATITUDE").ExecuteAction(sender, connection);

            currentLongitude = new GetVariable("PLANE LONGITUDE").ExecuteAction(sender, connection);
        }
    }
}
