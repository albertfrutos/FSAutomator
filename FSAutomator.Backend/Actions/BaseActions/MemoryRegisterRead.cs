﻿using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.Actions
{
    internal class MemoryRegisterRead : IAction
    {
        public bool RemoveAfterRead { get; set; }
        public string Id { get; set; }

        public MemoryRegisterRead()
        {

        }

        public MemoryRegisterRead(bool removeAfterRead, string id)
        {
            RemoveAfterRead = removeAfterRead;
            Id = id;
        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            var memoryRegisters = (sender as Automator).MemoryRegisters;

            if (memoryRegisters.Count == 0)
            {
                return new ActionResult($"No registers have been previously written", null, true);
            }

            var registers = memoryRegisters.Where(x => x.Key == this.Id).ToList();

            if (registers.Count == 0)
            {
                return new ActionResult($"No registers matching the Id provided have been found", null, true);
            }

            var selectedRegister = registers.First();

            if (this.RemoveAfterRead)
            {
                memoryRegisters.Remove(selectedRegister.Key);
            }

            return new ActionResult($"Read value is {selectedRegister.Value} with ID {this.Id}", selectedRegister.Value, false);
        }
    }
}
