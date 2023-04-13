using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.AutomatorInterface
{
    public class FSAutomatorInterfaceBaseActions
    {
        internal Automator automator { get; set; }
        internal ISimConnectBridge Connection { get; set; }
        private GeneralStatus Status { get; set; }

        IGetVariable getVariable;

        ISendEvent sendEvent;
        public FSAutomatorInterfaceBaseActions(Automator automator, ISimConnectBridge connection)
        {
            this.automator = automator;
            this.Connection = connection;
        }

        #region Interface Actions

        public dynamic GetVariable(string variableName, bool typedMode = false)
        {
            var action = new GetVariable(variableName);
            var result = action.ExecuteAction(automator, Connection);

            if (result.Error)
            {
                return result;
            }

            dynamic typedVariable = null;

            typedVariable = typedMode ? GetTypeVariableResult(variableName, result) : result;

            return typedVariable;
        }

        private static dynamic GetTypeVariableResult(string variableName, ActionResult result)
        {
            var variable = new Variable().GetVariableInformation(variableName);
            dynamic typedVariable = null;

            if (variable is not null && variable.Type is not null)
            {
                switch (variable.Type)
                {
                    case ("string"):
                        typedVariable = result.ComputedResult;
                        break;
                    case ("num"):
                        typedVariable = Convert.ToDouble(result.ComputedResult);
                        break;
                    case ("bool"):
                        typedVariable = result.ComputedResult == "0" ? false : true;
                        break;
                    default:
                        typedVariable = result.ComputedResult;
                        break;
                }
            }

            return typedVariable;
        }

        public ActionResult ExpectVariableValue(string variableName, string variableExpectedValue)
        {
            var action = new ExpectVariableValue(variableName, variableExpectedValue, getVariable);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult SendEvent(string eventName, string eventValue)
        {
            var action = new SendEvent(eventName, eventValue);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult WaitSeconds(int time)
        {
            var action = new WaitSeconds(time);
            var result = action.ExecuteAction(automator, Connection);
            return result;
        }

        public ActionResult WaitUntilVariableReachesNumericValue(string variableName, string comparison, string thresholdValue, int checkInterval = 200)
        {
            var action = new WaitUntilVariableReachesNumericValue(variableName, comparison, thresholdValue, new GetVariable(), checkInterval);
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