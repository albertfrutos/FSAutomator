using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Automators
{
    public class Automator
    {
        public SimConnect connection;

        public event EventHandler<string> NewReturnValue;
        public event EventHandler UnlockNextStep;

        AutoResetEvent StepLocker = new AutoResetEvent(false);

        public Dictionary<string, string> MemoryRegisters = new Dictionary<string, string>();
        public FlightModel flightModel;
        

        public string lastOperationValue = "";

        public ObservableCollection<FSAutomatorAction> ActionList;
        public ObservableCollection<FSAutomatorAction> AuxiliaryActionList;

        public Automator(ObservableCollection<FSAutomatorAction> ActionList)
        {
            this.ActionList = ActionList;
            this.AuxiliaryActionList = new ObservableCollection<FSAutomatorAction>();
        }

        public void Execute()
        {
            foreach (FSAutomatorAction action in ActionList)
            {
                RunAction(action);
            }

        }

        private void RunAction(FSAutomatorAction action)
        {
            action.Status = "Running";
            Trace.WriteLine("Executing");
            var res = action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { this, connection });
            //StepLocker.WaitOne();
            lastOperationValue = res.ToString();
            Trace.WriteLine("EXITING!!!!!!");
            action.Result = res.ToString();
            action.Status = "Done";
        }
    }
}
