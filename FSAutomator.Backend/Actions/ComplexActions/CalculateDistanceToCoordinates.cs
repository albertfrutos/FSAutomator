using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class CalculateDistanceToCoordinates : IAction
    {

        public double FinalLatitude { get; set; }
        public double FinalLongitude { get; set; }

        public EventHandler<string> getData;
        public EventHandler unlock;

        AutoResetEvent waiter = new AutoResetEvent(false);

        public string currentLatitude; 
        public string currentLongitude;

        public string receivedData = null;


        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
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
            double distance = GeoCalculator.GetDistance(origin, destination, 2, DistanceUnit.Kilometers);
            ReturnValueEvent.Invoke(this, distance.ToString());
            UnlockNextStep.Invoke(this, null);
        }

        private void GetCurrentCoordinates(object sender, SimConnect connection)
        {
            getData += ReceiveData;
            unlock += Unlock;

            new GetVariable("PLANE LATITUDE").ExecuteAction(sender, connection, getData, unlock);
            waiter.WaitOne();
            currentLatitude = receivedData;


            new GetVariable("PLANE LONGITUDE").ExecuteAction(sender, connection, getData, unlock);
            waiter.WaitOne();
            currentLongitude = receivedData;
        }

        private void Unlock(object? sender, EventArgs e)
        {
            waiter.Set();
        }

        private void ReceiveData(object? sender, string e)
        {
            receivedData = e;
        }



    }
}
