using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class SetAP : IAction
    {

        public string APStatus { get; set; }

        public EventHandler<string> getData;
        public EventHandler unlock;

        AutoResetEvent waiter = new AutoResetEvent(false);

        public string currentLatitude; 
        public string currentLongitude;

        public string receivedData = null;


        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            var apCurrentStatus = GetCurrentAPStatus(sender, connection);

            if (apCurrentStatus != APStatus)
            {
                new SendEvent("AP_MASTER", "1");
            }

            var newAPStatus = GetCurrentAPStatus(sender, connection);

            ReturnValueEvent.Invoke(this, newAPStatus);
            UnlockNextStep.Invoke(this, null);
        }

        private string GetCurrentAPStatus(object sender, SimConnect connection)
        {
            getData += ReceiveData;
            unlock += Unlock;

            new GetVariable("AUTOPILOT MASTER").ExecuteAction(sender, connection, getData, unlock);
            waiter.WaitOne();
            return receivedData;
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
