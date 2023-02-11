using Microsoft.FlightSimulator.SimConnect;
using FSAutomator.Backend.Actions;
using System.Collections.ObjectModel;
using FSAutomator.Backend.Entities;
using System.Diagnostics;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public string Execute(object sender, SimConnect connection, AutoResetEvent evento)
        {
            // To get here, you need to execute the automation as dll.
            //new SendEvent("HEADING_BUG_SET", "25").ExecuteAction(this, connection, MainReturnValueEvent, MainUnlockNextStep);
            Trace.WriteLine("test ExecuteTest");

            //new GetVariable("ATC ID").ExecuteAction(this, connection, MainReturnValueEvent, MainUnlockNextStep);
            evento.Set();
            return "finish exe.";
        }

        public string MyLonelyMethod(object sender, SimConnect connection, AutoResetEvent evento, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            // To get here, you need to execute the automation as dll.
            //new SendEvent("HEADING_BUG_SET", "25").ExecuteAction(this, connection, MainReturnValueEvent, MainUnlockNextStep);

            Trace.WriteLine("test MyLonelyMethodTest");
            evento.Set();
            return "finish mlm.";
        }
    }
    public class Lalala
    {
        public string LalalaTest(object sender, SimConnect connection, AutoResetEvent evento, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            Trace.WriteLine("test lalalatest");
            evento.Set();
            return "finish lalala.";
        }
    }
}
