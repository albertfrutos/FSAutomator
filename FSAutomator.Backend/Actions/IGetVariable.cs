using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.Actions
{
    public interface IGetVariable
    {
        public string VariableName { get; set; }
        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection);
    }
}
