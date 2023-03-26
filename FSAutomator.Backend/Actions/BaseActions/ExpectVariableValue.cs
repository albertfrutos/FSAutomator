using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue : ActionBase, IAction
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }




        internal ExpectVariableValue(string variableName, string variableExpectedValue, IGetVariable getVariable) : base(getVariable, variableName)
        {
            VariableName = variableName;
            VariableExpectedValue = variableExpectedValue;
        }

        public ExpectVariableValue():base()
        {

        }
        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var result = this.getVariable.ExecuteAction(sender, connection);

            string isExpectedValue = CheckIfVariableHasExpectedValue(sender, connection, result.ComputedResult);

             return new ActionResult(isExpectedValue, isExpectedValue, result.Error);
        }

        internal string CheckIfVariableHasExpectedValue(object sender, SimConnect connection, string variableRealValue)
        {
            this.VariableExpectedValue = Utils.GetValueToOperateOnFromTag(sender, connection, this.VariableExpectedValue);

            var isExpectedValue = (variableRealValue == VariableExpectedValue).ToString();
            return isExpectedValue;
        }
    }
}
