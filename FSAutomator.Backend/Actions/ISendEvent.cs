using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.Actions
{
    public interface ISendEvent
    {
        public string EventName { get; set; }
        public string EventValue { get; set; }
        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection);
    }
}
