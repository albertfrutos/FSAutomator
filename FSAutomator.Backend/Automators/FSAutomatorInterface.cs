using FSAutomator.Backend;
using FSAutomator.Backend.Actions;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Automators
{
    public class FSAutomatorInterface
    {
        private Automator Automator { get; }
        private SimConnect Connection { get; }

        private AutoResetEvent FinishEvent { get; }


        public FSAutomatorInterface(Automator automator, SimConnect connection, AutoResetEvent finishEvent)
        {
            Automator = automator;
            Connection = connection;
            FinishEvent = finishEvent;
        }



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

        public ActionResult TextTest (string text)
        {
            return new ActionResult(text,text,false);
        }

        public void EndAutomation()
        {
            FinishEvent.Set();
            return;
        }
        

    }
}