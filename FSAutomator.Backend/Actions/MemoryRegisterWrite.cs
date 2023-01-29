using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterWrite
    {
        public string Value { get; set; }
        public string Id { get; set; }

        
        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
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

            ReturnValueEvent.Invoke(this, this.Id);
            UnlockNextStep.Invoke(this, null);
            
        }
    }
}
