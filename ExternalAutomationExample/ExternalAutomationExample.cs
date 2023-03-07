using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public ActionResult Execute(FSAutomatorInterface FSAutomator)
        {
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

            FSAutomator.AutomationHasEnded();

            return new ActionResult("Finished successfully.", null, false);
        }

        public ActionResult MyLonelyMethod(object sender, SimConnect connection, AutoResetEvent evento, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            Trace.WriteLine("test MyLonelyMethodTest");
            evento.Set();
            return new ActionResult("finish mlm.", "OK", false);
        }
    }

    public class Lalala
    {
        public ActionResult LalalaTest(object sender, SimConnect connection, AutoResetEvent evento, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            Trace.WriteLine("test lalalatest");
            evento.Set();
            return new ActionResult("finish lalala.", "OK", false);
        }
    }
}
