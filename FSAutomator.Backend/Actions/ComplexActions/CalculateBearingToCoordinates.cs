using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
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

        private AutomationFile automationFile;



        public ActionResult ExecuteAction(object sender, SimConnect connection, AutomationFile automationFile)
        {
            this.automationFile = automationFile;
            GetCurrentCoordinates(sender, connection);

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

            return new ActionResult(heading.ToString(), heading.ToString());
        }

        private void GetCurrentCoordinates(object sender, SimConnect connection)
        {
            currentLatitude = new GetVariable("PLANE LATITUDE").ExecuteAction(sender, connection, automationFile).ComputedResult;

            currentLongitude = new GetVariable("PLANE LONGITUDE").ExecuteAction(sender, connection, automationFile).ComputedResult;
        }
    }
}
