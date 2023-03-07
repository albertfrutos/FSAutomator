using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.AutomatorInterface
{
    public class FSAutomatorInterfaceBaseActions
    {
        internal Automator automator { get; set; }
        internal SimConnect Connection { get; set; }
        private GeneralStatus Status { get; set; }

        public FSAutomatorInterfaceBaseActions(Automator automator, SimConnect connection)
        {
            this.automator = automator;
            this.Connection = connection;
        }

        /*
        #region Interface Events

        private void NotifyConnectionStatusChange(object sender, bool connectionStatus)
        {
            this.ConnectionStatusChangeEvent.Invoke(this, connectionStatus);
        }

        public void ReportStatus(object sender, InternalMessage msg)
        {
            this.ReportErrorEvent.Invoke(this, msg);
        }

        #endregion
        */

        #region Interface Actions

        public ActionResult GetVariable(string variableName)
        {
            var action = new GetVariable(variableName);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult ExpectVariableValue(string variableName, string variableExpectedValue)
        {
            var action = new ExpectVariableValue(variableName, variableExpectedValue);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult SendEvent(string eventName, string eventValue)
        {
            var action = new SendEvent(eventName, eventValue);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult WaitSeconds(string time)
        {
            var action = new WaitSeconds(time);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult WaitUntilVariableReachesNumericValue(string variableName, string comparison, string thresholdValue, int checkInterval = 200)
        {
            var action = new WaitUntilVariableReachesNumericValue(variableName, comparison, thresholdValue, checkInterval);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }


        public ActionResult TextTest(string visible, string computed, bool isError)
        {
            return new ActionResult(visible, computed, isError);
        }

        public bool IsConnectedToSim()
        {
            return Status.IsConnectedToSim;
        }


        #endregion
    }
}