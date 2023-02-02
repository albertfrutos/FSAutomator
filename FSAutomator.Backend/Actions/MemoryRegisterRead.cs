using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterRead
    {
        public bool RemoveAfterRead { get; set; }
        public string Id { get; set; }
        public string ExecuteAction(object sender, SimConnect connection)
        {
            var memoryRegisters = (Dictionary<string,string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);

            var selectedRegister = memoryRegisters.Where(x => x.Key == this.Id).First();

            if (this.RemoveAfterRead)
            {
                memoryRegisters.Remove(selectedRegister.Key);
            }

            return selectedRegister.Value;
        }
    }
}
