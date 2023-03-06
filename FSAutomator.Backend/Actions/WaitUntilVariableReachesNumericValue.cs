using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;

namespace FSAutomator.Backend.Actions
{
    public class WaitUntilVariableReachesNumericValue : IAction
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

        internal WaitUntilVariableReachesNumericValue(string variableName, string comparison, string thresholdValue, int checkInterval = 200)
        {
            VariableName = variableName;
            Comparison = comparison;
            ThresholdValue = thresholdValue;
            CheckInterval = checkInterval;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            if (!AllowedComparisonValues.Contains(Comparison))
            {
                return new ActionResult($"Comparison not supported - {this.Comparison}", null, true);
            }

            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);

            if (actionsList != null)
            {
                CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();
            }

            this.ThresholdValue = Utils.GetValueToOperateOnFromTag(sender, connection, this.ThresholdValue);

            if (!Utils.IsNumericDouble(this.ThresholdValue))
            {
                return new ActionResult($"ThresholdValue not a number - {this.ThresholdValue}", null, true);
            }

            do
            {
                var variableResult = new GetVariable(this.VariableName).ExecuteAction(sender, connection).ComputedResult;
                CheckVariableRecovered(variableResult);
                Thread.Sleep(CheckInterval);
            } while (!this.isValueReached);

            return new ActionResult($"Accomplished - {this.variableValue}", this.variableValue);


        }

        private void CheckVariableRecovered(string variable)
        {
            var currentValue = Convert.ToDouble(variable);
            var thresHoldValue = Convert.ToDouble(this.ThresholdValue);
            this.variableValue = variable;
            if (CurrentAction != null)
            {
                CurrentAction.Result.VisibleResult = String.Format("Current value - {0}", this.variableValue);
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
