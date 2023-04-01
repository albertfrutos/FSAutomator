using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterWrite
    {
        public string Value { get; set; }
        public string Id { get; set; }

        public MemoryRegisterWrite()
        {

        }

        public MemoryRegisterWrite(string value, string id)
        {
            Value = value;
            Id = id;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            this.Value = Utils.GetValueToOperateOnFromTag(sender, connection, this.Value);

            var memoryRegisters = (sender as Automator).MemoryRegisters;

            this.Id = String.IsNullOrEmpty(this.Id) ? Guid.NewGuid().ToString() : this.Id;

            if (memoryRegisters.ContainsKey(this.Id))
            {
                return new ActionResult($"A register with id {this.Id} already exists.", null, true);
            }

            memoryRegisters.Add(this.Id, this.Value);

            return new ActionResult($"Written data with id {this.Id}", this.Value, false);
        }
    }
}
