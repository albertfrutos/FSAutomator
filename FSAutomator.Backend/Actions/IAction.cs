using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    interface IAction
    {
        public ActionResult ExecuteAction(object sender, SimConnect connection);
    }
}
