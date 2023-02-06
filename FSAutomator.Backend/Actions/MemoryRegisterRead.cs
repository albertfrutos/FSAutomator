using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterRead : IAction
    {
        public bool RemoveAfterRead { get; set; }
        public string Id { get; set; }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var memoryRegisters = (Dictionary<string,string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);

            if (memoryRegisters.Count == 0)
            {
                return new ActionResult($"No registers have been previously written", null, true);
            }

            var registers = memoryRegisters.Where(x => x.Key == this.Id).ToList();
            
            if(registers.Count == 0)
            {
                return new ActionResult($"No registers matching the Id provided have been found", null, true);
            }

            var selectedRegister = registers.First();

            if (this.RemoveAfterRead)
            {
                memoryRegisters.Remove(selectedRegister.Key);
            }

            return new ActionResult($"Read value is {selectedRegister.Value} with ID {this.Id}", selectedRegister.Value);
        }
    }
}
