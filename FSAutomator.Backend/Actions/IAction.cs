using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    interface IAction
    {
        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep);
    }
}
