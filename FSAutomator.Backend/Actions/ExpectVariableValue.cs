using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class ExpectVariableValue
    {

        public string VariableName { get; set; }
        public string VariableExpectedValue { get; set; }


        internal string IsVariableTheExpectedValue;


        public string ExecuteAction(object sender, SimConnect connection)
        {
            var result = new GetVariable(this.VariableName).ExecuteAction(sender, connection);

            this.IsVariableTheExpectedValue = (result == VariableExpectedValue).ToString();

            return this.IsVariableTheExpectedValue;
        }

    }
}
