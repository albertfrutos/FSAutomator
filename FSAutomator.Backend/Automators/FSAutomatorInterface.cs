using FSAutomator.Backend;
using FSAutomator.Backend.Actions;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics;

namespace FSAutomator.Backend.Automators
{
    public class FSAutomatorInterface
    {
        private Automator Automator { get; }
        private SimConnect Connection { get; }
        private AutoResetEvent FinishEvent { get; }
        private GeneralStatus Status { get; set; }

        public event EventHandler<bool> ConnectionStatusChangeEvent;

        public event EventHandler<string> ReportErrorEvent;



        public FSAutomatorInterface(Automator automator, SimConnect connection, AutoResetEvent finishEvent, GeneralStatus status)
        {
            this.Automator = automator;
            this.Connection = connection;
            this.FinishEvent = finishEvent;
            this.Status = status;
            Status.ConnectionStatusChangeEvent += NotifyConnectionStatusChange;
            Status.ReportErrorEvent += ReportError;
        }

        #region Interface Events

        private void NotifyConnectionStatusChange(object sender, bool connectionStatus)
        {
            Trace.WriteLine("csc int");
            this.ConnectionStatusChangeEvent.Invoke(this, connectionStatus);
        }

        private void ReportError(object sender, string msg)
        {
            Trace.WriteLine("ReportError");
            this.ReportErrorEvent.Invoke(this, msg);
        }

        #endregion

        #region Interface Actions

        public ActionResult GetVariable(string variableName)
        {
            var action = new GetVariable(variableName);
            var result = action.ExecuteAction(Automator, Connection);
            return result;
        }

        public ActionResult ExpectVariableValue(string variableName, string variableExpectedValue)
        {
            var action = new ExpectVariableValue(variableName, variableExpectedValue);
            var result = action.ExecuteAction(Automator, Connection);
            return result;
        }

        public ActionResult SendEvent(string eventName, string eventValue)
        {
            var action = new SendEvent(eventName, eventValue);
            var result = action.ExecuteAction(Automator, Connection);
            return result;
        }

        public ActionResult WaitSeconds(string time)
        {
            var action = new WaitSeconds(time);
            var result = action.ExecuteAction(Automator, Connection);
            return result;
        }

        public ActionResult WaitUntilVariableReachesNumericValue(string variableName, string comparison, string thresholdValue, int checkInterval = 200)
        {
            var action = new WaitUntilVariableReachesNumericValue(variableName, comparison, thresholdValue, checkInterval);
            var result = action.ExecuteAction(Automator, Connection);
            return result;
        }

        #endregion

        public ActionResult TextTest (string text)
        {
            
            return new ActionResult(text,text,false);
        }

        public void AutomationHasEnded()
        {
            FinishEvent.Set();
            return;
        }

        public bool IsConnectedToSim()
        {
            Status.IsConnectedToSim = true;
            return Status.IsConnectedToSim;
        }

    }
}