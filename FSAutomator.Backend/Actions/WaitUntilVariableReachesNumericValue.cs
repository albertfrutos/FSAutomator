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

        internal FlightModel fm;

        private string variableValue = string.Empty;

        public event EventHandler<string> ReportInternalVariableValueEvent;
        EventHandler<string> ReturnValueEvent;
        AutoResetEvent LockVariableCheck = new AutoResetEvent(false);

        internal FSAutomatorAction? CurrentAction = null;

        bool isValueReached = false;

        SimConnect connection = null;

        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            this.connection = connection;
            this.ReturnValueEvent = ReturnValueEvent;

            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("l_ActionList").GetValue(sender);
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
                    ReturnValueEvent.Invoke(this, String.Format("{0} not found in the flight model", ThresholdValue));
                    UnlockNextStep.Invoke(this, null);
                    return;
                }
            }

            if (!Utils.IsNumericDouble(this.ThresholdValue))
            {
                ReturnValueEvent.Invoke(this, String.Format("ThresholdValue not a number - {0}", this.ThresholdValue));
                UnlockNextStep.Invoke(this, null);
                return;
            }

            /*
            MonitoredVariable = new GetVariable()
            {
                VariableName = this.VariableName
            };
            */

            ReportInternalVariableValueEvent += CheckVariableRecovered;

            do
            {
                new GetVariable(this.VariableName).ExecuteAction(sender, connection, ReportInternalVariableValueEvent, UnlockNextStep);
                LockVariableCheck.WaitOne();
                Thread.Sleep(CheckInterval); // NOTE : do it with a Timer
            } while(!this.isValueReached);

            ReturnValueEvent.Invoke(this, String.Format("Accomplished - {0}", this.variableValue));
            UnlockNextStep.Invoke(this, null);

        }

        private void CheckVariableRecovered(object? sender, string e)
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

            LockVariableCheck.Set();


        }


    }
}
