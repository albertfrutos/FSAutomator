using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.Actions
{
    interface IAction
    {
        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection);
    }
}
