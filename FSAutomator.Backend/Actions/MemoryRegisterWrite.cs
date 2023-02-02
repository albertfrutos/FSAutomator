using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterWrite
    {
        public string Value { get; set; }
        public string Id { get; set; }

        
        public string ExecuteAction(object sender, SimConnect connection)
        {

            if (this.Value == "%PrevValue%")
            {
                this.Value = sender.GetType().GetField("lastOperationValue").GetValue(sender).ToString();
            }

            if (String.IsNullOrEmpty(Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }

            var memoryRegisters = (Dictionary<string,string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);
            memoryRegisters.Add(this.Id, this.Value);

            return this.Id;            
        }
    }
}
