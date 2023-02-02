using FSAutomator.BackEnd.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class SetAP : IAction
    {

        public string APStatus { get; set; }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var apCurrentStatus = GetCurrentAPStatus(sender, connection);

            if (apCurrentStatus != APStatus)
            {
                new SendEvent("AP_MASTER", "1");
            }

            var newAPStatus = GetCurrentAPStatus(sender, connection);

            return new ActionResult($"Final AP status: {newAPStatus}", newAPStatus);

        }

        private string GetCurrentAPStatus(object sender, SimConnect connection)
        {

            var receivedData = new GetVariable("AUTOPILOT MASTER").ExecuteAction(sender, connection).ComputedResult;
            
            return receivedData;
        }





    }
}
