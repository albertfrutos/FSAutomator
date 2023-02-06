using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Entities;
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

        internal FlightModel fm;

        private string variableValue = string.Empty;

        internal FSAutomatorAction? CurrentAction = null;

        bool isValueReached = false;

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

            var valueToOperate = Utils.GetValueToOperateOnFromTag(sender, connection, this.ThresholdValue);

            if (!Utils.IsNumericDouble(valueToOperate))
            {
                return new ActionResult($"ThresholdValue not a number - {valueToOperate}", null, true);
            }

            do
            {
                var variableResult = new GetVariable(this.VariableName).ExecuteAction(sender, connection).ComputedResult;
                CheckVariableRecovered(variableResult);
                Thread.Sleep(CheckInterval); // NOTE : do it with a Timer
            } while(!this.isValueReached);

            return new ActionResult($"Accomplished - {this.variableValue}", this.variableValue);


        }

        private void CheckVariableRecovered(string variable)
        {
            var currentValue = Convert.ToDouble(variable);
            var thresHoldValue = Convert.ToDouble(this.ThresholdValue);
            this.variableValue = variable;
            CurrentAction.Result.VisibleResult = String.Format("Current value - {0}", this.variableValue);

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
