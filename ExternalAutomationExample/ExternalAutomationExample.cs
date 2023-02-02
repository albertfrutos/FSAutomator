using Microsoft.FlightSimulator.SimConnect;
using FSAutomator.Backend.Actions;
using System.Collections.ObjectModel;
using FSAutomator.Backend.Entities;
using System.Diagnostics;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public void Execute(object sender, SimConnect connection)
        {
            // To get here, you need to execute the automation as dll.
            //new SendEvent("HEADING_BUG_SET", "25").ExecuteAction(this, connection, MainReturnValueEvent, MainUnlockNextStep);
            Trace.WriteLine("test ExecuteTest");
            //new GetVariable("ATC ID").ExecuteAction(this, connection, MainReturnValueEvent, MainUnlockNextStep);
        }

        public void MyLonelyMethod(object sender, SimConnect connection, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            // To get here, you need to execute the automation as dll.
            //new SendEvent("HEADING_BUG_SET", "25").ExecuteAction(this, connection, MainReturnValueEvent, MainUnlockNextStep);

            Trace.WriteLine("test MyLonelyMethodTest");
        }
    }
    public class Lalala
    {
        public void LalalaTest(object sender, SimConnect connection, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            Trace.WriteLine("test lalalatest");
        }
    }
}
