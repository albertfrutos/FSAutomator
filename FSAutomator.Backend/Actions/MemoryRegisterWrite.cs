using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterWrite
    {
        public string Value { get; set; }
        public string Id { get; set; }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            this.Value = Utils.GetValueToOperateOnFromTag(this, connection, this.Value);

            if (String.IsNullOrEmpty(Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }

            var memoryRegisters = (Dictionary<string,string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);
            memoryRegisters.Add(this.Id, this.Value);

            return new ActionResult($"Written data with id {this.Id}", this.Value);            
        }
    }
}
