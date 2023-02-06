using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue : IAction
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }

        internal string IsVariableTheExpectedValue;


        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var result = new GetVariable(this.VariableName).ExecuteAction(sender, connection).ComputedResult;

            this.IsVariableTheExpectedValue = (result == VariableExpectedValue).ToString();

            return new ActionResult(this.IsVariableTheExpectedValue, this.IsVariableTheExpectedValue);
        }

    }
}
