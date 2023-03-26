using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public interface IGetVariable
    {
        public string VariableName { get; set; }
        public ActionResult ExecuteAction(object sender, SimConnect connection);
    }
}
