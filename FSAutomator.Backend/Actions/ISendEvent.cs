using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public interface ISendEvent
    {
        public string EventName { get; set; }
        public string EventValue { get; set; }
        public ActionResult ExecuteAction(object sender, SimConnect connection);
    }
}
