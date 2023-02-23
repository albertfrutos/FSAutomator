using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using FSAutomator.Backend.Entities;
using System.Diagnostics;
using FSAutomator.Backend.Automators;
using FSAutomator.BackEnd.Entities;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public string Execute(FSAutomatorInterface FSAutomator)
        {
            Trace.WriteLine("test ExecuteTest");

            var a = FSAutomator.TextTest("abcd").VisibleResult;
            FSAutomator.ConnectionStatusChangeEvent += ConnectionChangeStatusReceiver;
            FSAutomator.ReportErrorEvent += ReportErrorReceiver;

            FSAutomator.IsConnectedToSim();
            
            FSAutomator.AutomationHasEnded();
            return "finish exe.";
        }

        private void ReportErrorReceiver(object? sender, InternalMessage e)
        {
            Trace.WriteLine("launched report error  -  " + e);
        }

        private void ConnectionChangeStatusReceiver(object? sender, bool e)
        {
            Trace.WriteLine("launched changed status conenction  - " + e);
        }

        public string MyLonelyMethod(object sender, SimConnect connection, AutoResetEvent evento, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        // note fer un objecte amb tot això per evitar tenir-ho que passar.
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
