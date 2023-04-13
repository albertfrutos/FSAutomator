using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    interface IAction
    {
        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection);
    }
}
