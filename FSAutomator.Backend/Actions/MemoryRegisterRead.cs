using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterRead
    {
        public bool RemoveAfterRead { get; set; }
        public string Id { get; set; }
        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            var memoryRegisters = (Dictionary<string,string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);

            var selectedRegister = memoryRegisters.Where(x => x.Key == this.Id).First();

            if (this.RemoveAfterRead)
            {
                memoryRegisters.Remove(selectedRegister.Key);
            }

            ReturnValueEvent.Invoke(this, selectedRegister.Value);
            UnlockNextStep.Invoke(this, null);
        }
    }
}
