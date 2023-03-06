using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue : IAction
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }


        internal ExpectVariableValue(string variableName, string variableExpectedValue)
        {
            VariableName = variableName;
            VariableExpectedValue = variableExpectedValue;
        }

        public ExpectVariableValue()
        {

        }
        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var result = new GetVariable(this.VariableName).ExecuteAction(sender, connection).ComputedResult;

            this.VariableExpectedValue = Utils.GetValueToOperateOnFromTag(sender, connection, this.VariableExpectedValue);

            var isExpectedValue = (result == VariableExpectedValue).ToString();

            return new ActionResult(isExpectedValue, isExpectedValue);
        }

    }
}
