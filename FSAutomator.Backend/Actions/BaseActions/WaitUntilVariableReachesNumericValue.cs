using FSAutomator.Backend.Actions.Base;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;

namespace FSAutomator.Backend.Actions
{
    public class WaitUntilVariableReachesNumericValue : ActionBase, IAction
    {

        public string VariableName { get; set; }
        public string Comparison { get; set; }
        public string ThresholdValue { get; set; }
        public int CheckInterval { get; set; }

        internal string[] AllowedComparisonValues = { "<", ">", "=" };

        private string variableValue = string.Empty;

        internal FSAutomatorAction CurrentAction = null;

        bool isValueReached = false;

        public WaitUntilVariableReachesNumericValue()
        {

        }

        internal WaitUntilVariableReachesNumericValue(string variableName, string comparison, string thresholdValue, IGetVariable getVariable, int checkInterval = 200) :base(getVariable)
        {
            VariableName = variableName;
            Comparison = comparison;
            ThresholdValue = thresholdValue;
            CheckInterval = checkInterval;
            this.getVariable = getVariable;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            if (!AllowedComparisonValues.Contains(Comparison))
            {
                return new ActionResult($"Comparison not supported - {this.Comparison}", null, true);
            }

            this.GetCurrentAction(sender);

            if (this.CurrentAction == null)
            {
                return new ActionResult("No current action could be found", null, true);
            }

            this.ThresholdValue = Utils.GetValueToOperateOnFromTag(sender, connection, this.ThresholdValue);

            if (!Utils.IsNumericDouble(this.ThresholdValue))
            {
                return new ActionResult($"ThresholdValue not a number - {this.ThresholdValue}", null, true);
            }

            do
            {
                var variableResult = getVariable.ExecuteAction(sender, connection).ComputedResult;
                CheckVariableRecovered(variableResult);
                Thread.Sleep(CheckInterval);
            } while (!this.isValueReached);

            return new ActionResult($"Value reached: {this.variableValue}", this.variableValue);


        }

        private void GetCurrentAction(object sender)
        {
            var actionsList = (sender as Automator).ActionList;

            if (actionsList != null)
            {
                CurrentAction = actionsList.Where(x => x.Status == FSAutomatorAction.ActionStatus.Running).FirstOrDefault();
            }
        }

        private void CheckVariableRecovered(string variable)
        {
            var currentValue = Convert.ToDouble(variable);
            var thresHoldValue = Convert.ToDouble(this.ThresholdValue);
            this.variableValue = variable;
            if (CurrentAction != null)
            {
                CurrentAction.Result.VisibleResult = String.Format($"Value for {this.VariableName} : {this.variableValue} - Target: {this.Comparison}{this.ThresholdValue}");
            }

            bool result = false;

            switch (Comparison)
            {
                case "<":
                    result = currentValue < thresHoldValue;
                    break;
                case ">":
                    result = currentValue > thresHoldValue;
                    break;
                case "=":
                    result = currentValue == thresHoldValue;
                    break;
                default:
                    result = true;
                    break;

            }

            this.isValueReached = result;



        }


    }
}
