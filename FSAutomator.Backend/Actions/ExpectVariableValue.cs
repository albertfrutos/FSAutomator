using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue : IAction
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }

        internal string IsVariableTheExpectedValue;


        public ActionResult ExecuteAction(object sender, SimConnect connection, AutomationFile automationFile)
        {
            var result = new GetVariable(this.VariableName).ExecuteAction(sender, connection, automationFile).ComputedResult;

            this.IsVariableTheExpectedValue = (result == VariableExpectedValue).ToString();

            return new ActionResult(this.IsVariableTheExpectedValue, this.IsVariableTheExpectedValue);
        }

    }
}
