using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public string Execute(FSAutomatorInterface FSAutomator)
        {
            //FSAutomator.Status.ConnectionStatusChangeEvent += ConnectionChangeStatusReceiver;
            //FSAutomator.ReportErrorEvent += ReportErrorReceiver;

            var AP = FSAutomator.APManager;
            var AA = FSAutomator.AAManager;
            var hdgReus = AA.CalculateBearingToCoordinates("41.176307", "1.262329");

            AP.SetEventAutopilotOn("1");
            var hdg = FSAutomator.GetVariable("PLANE HEADING DEGREES GYRO");
            AP.SetEventHeadingBugSet(hdg.ComputedResult);
            AP.SetEventApHdgHoldOn("0");
            AP.SendEvent("PARKING_BRAKE_SET", "0");
            AP.SendEvent("THROTTLE_FULL", "0");
            AP.WaitUntilVariableReachesNumericValue("GROUND VELOCITY", ">", "150", 200);
            AP.SetEventApPanelVsOn("1");
            AP.SetEventApVsVarSetEnglish("1500");
            var alt = AP.GetVariable("PLANE ALTITUDE").ComputedResult;
            var newAlt = Convert.ToDouble(alt) + 1000;
            AP.WaitUntilVariableReachesNumericValue("PLANE ALTITUDE", ">", newAlt.ToString(), 200);
            AP.SendEvent("GEAR_UP", "1");
            AP.SetEventApVsVarSetEnglish("1500");

            AP.SendEvent("HEADING_BUG_SET", hdgReus);









            //var isCd = FSAutomator.automatorInterfaceBaseActions.IsConnectedToSim();

            //FSAutomator.automatorInterfaceBaseActions.ReportError(this, new InternalMessage("a", "b", true));

            //var hdg = ap.GetHeading();

            FSAutomator.AutomationHasEnded();
            return "finish exe.";

            // var a = FSAutomator.TextTest("abcdV", "abcdC", true).VisibleResult;
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
