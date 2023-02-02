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

        SimConnect connection = null;

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            this.connection = connection;

            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

            if (ThresholdValue == "%PrevValue%")
            {
                ThresholdValue = sender.GetType().GetField("lastOperationValue").GetValue(sender).ToString();
            }
            else if (ThresholdValue.StartsWith('%'))
            {
                FlightModel fm = (FlightModel)sender.GetType().GetField("flightModel").GetValue(sender);
                var property =   fm.ReferenceSpeeds.GetType().GetProperty(ThresholdValue.Replace("%", ""));

                if (property != null)
                {
                    ThresholdValue = (string)property.GetValue(fm.ReferenceSpeeds);
                }
                else
                {
                    return new ActionResult($"{ThresholdValue} not found in the flight model", null);                    
                }
            }

            if (!Utils.IsNumericDouble(this.ThresholdValue))
            {
                return new ActionResult($"ThresholdValue not a number - {this.ThresholdValue}", null);                
            }

            //ReportInternalVariableValueEvent += CheckVariableRecovered;

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
            CurrentAction.Result = String.Format("Current value - {0}", this.variableValue);
            //NOTE : sthetic topic: "acccomplished - curren value -- XXX"

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
            }

            this.isValueReached = result;



        }


    }
}
