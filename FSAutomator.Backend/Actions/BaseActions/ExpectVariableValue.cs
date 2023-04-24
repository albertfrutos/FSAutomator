using FSAutomator.Backend.Actions.Base;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue : ActionBase, IAction
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }

        internal ExpectVariableValue(string variableName, string variableExpectedValue, IGetVariable getVariable) : base(getVariable)
        {
            VariableName = variableName;
            VariableExpectedValue = variableExpectedValue;
            this.getVariable = getVariable;
        }

        public ExpectVariableValue() : base()
        {

        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            var result = this.getVariable.ExecuteAction(sender, connection);

            string isExpectedValue = CheckIfVariableHasExpectedValue(sender, connection, result.ComputedResult);

            return new ActionResult(isExpectedValue, isExpectedValue, result.Error);
        }

        private string CheckIfVariableHasExpectedValue(object sender, ISimConnectBridge connection, string variableRealValue)
        {
            this.VariableExpectedValue = Utils.GetValueToOperateOnFromTag(sender, connection, this.VariableExpectedValue);

            var isExpectedValue = (variableRealValue == VariableExpectedValue).ToString();

            return isExpectedValue;
        }
    }
}
