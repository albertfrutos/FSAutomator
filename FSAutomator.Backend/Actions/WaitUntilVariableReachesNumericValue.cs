using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;

namespace FSAutomator.Backend.Actions
{
    public class WaitUntilVariableReachesNumericValue
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

        public string ExecuteAction(object sender, SimConnect connection)
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
                    return String.Format("{0} not found in the flight model", ThresholdValue);                    
                }
            }

            if (!Utils.IsNumericDouble(this.ThresholdValue))
            {
                return String.Format("ThresholdValue not a number - {0}", this.ThresholdValue);                
            }

            //ReportInternalVariableValueEvent += CheckVariableRecovered;

            do
            {
                var res = new GetVariable(this.VariableName).ExecuteAction(sender, connection);
                CheckVariableRecovered(res);
                Thread.Sleep(CheckInterval); // NOTE : do it with a Timer
            } while(!this.isValueReached);

            return String.Format("Accomplished - {0}", this.variableValue);


        }

        private void CheckVariableRecovered(string e)
        {
            var currentValue = Convert.ToDouble(e);
            var thresHoldValue = Convert.ToDouble(this.ThresholdValue);
            this.variableValue = e;
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
