using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;

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

        public Automator(ObservableCollection<FSAutomatorAction> ActionList)
        {
            this.ActionList = ActionList;
        }

        public void Execute()
        {
            NewReturnValue += GetUpdatedReturnValue;
            UnlockNextStep += UnlockStep;


            foreach (FSAutomatorAction action in ActionList)
            {
                RunAction(action);
            }

        }

        private void RunAction(FSAutomatorAction action)
        {
            action.Status = "Running";
            action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { this, connection, NewReturnValue, UnlockNextStep });
            StepLocker.WaitOne();
            action.Result = lastOperationValue;
            action.Status = "Done";
        }

        private void UnlockStep(object? sender, EventArgs e)
        {
            StepLocker.Set();
        }

        private void GetUpdatedReturnValue(object? sender, string e)
        {
            lastOperationValue = e;
        }
    }
}
