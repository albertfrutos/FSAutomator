using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue : IAction
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }

        public event EventHandler<string> ReportInternalVariableValueEvent;
        AutoResetEvent LockVariableCheck = new AutoResetEvent(false);

        internal string IsVariableTheExpectedValue;


        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {

            ReportInternalVariableValueEvent += IsValueExpected;

            new GetVariable(this.VariableName).ExecuteAction(sender, connection, ReportInternalVariableValueEvent, UnlockNextStep);

            LockVariableCheck.WaitOne();

            ReturnValueEvent.Invoke(this, IsVariableTheExpectedValue);
            UnlockNextStep.Invoke(this, null);
        }

        private void IsValueExpected(object? sender, string e)
        {
            this.IsVariableTheExpectedValue = (e == VariableExpectedValue).ToString();
            LockVariableCheck.Set();
        }
    }
}
