using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.ExternalAutomation
{
    public class ExternalAutomation
    {
        public string Execute(FSAutomatorInterface FSAutomator)
        {
            var Autopilot = FSAutomator.AutoPilotManager;
            var AdvancedActions = FSAutomator.AdvancedActionsManager;

            var initialPlaneHeading = FSAutomator.GetVariable("PLANE HEADING DEGREES GYRO");

            var initialAltitude = Autopilot.GetVariable("PLANE ALTITUDE").ComputedResult;
            var retractLandingGearAltitude = Convert.ToDouble(initialAltitude) + 1000;

            Autopilot.SetEventAutopilotOn("1");
            Autopilot.SetEventHeadingBugSet(initialPlaneHeading.ComputedResult);
            Autopilot.SetEventApHdgHoldOn("0");
            Autopilot.SendEvent("PARKING_BRAKE_SET", "0");
            Autopilot.SendEvent("THROTTLE_FULL", "0");
            Autopilot.WaitUntilVariableReachesNumericValue("GROUND VELOCITY", ">", "150", 200);
            Autopilot.SetEventApPanelVsOn("1");
            Autopilot.SetEventApVsVarSetEnglish("1500");
            Autopilot.WaitUntilVariableReachesNumericValue("PLANE ALTITUDE", ">", retractLandingGearAltitude.ToString(), 200);
            Autopilot.SendEvent("GEAR_UP", "1");
            Autopilot.SetEventApVsVarSetEnglish("1500");

            var headingToReusLERS = AdvancedActions.CalculateBearingToCoordinates("41.176307", "1.262329");
            Autopilot.SendEvent("HEADING_BUG_SET", headingToReusLERS);

            FSAutomator.AutomationHasEnded();

            return "Finished execution";
        }

        public string MyLonelyMethod(object sender, SimConnect connection, AutoResetEvent finishEvent, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            Trace.WriteLine("test MyLonelyMethodTest");
            finishEvent.Set();
            return "finish mlm.";
        }
    }

    public class Lalala
    {
        public string LalalaTest(object sender, SimConnect connection, AutoResetEvent finishEvent, Dictionary<string, string> memoryRegisters, string lastValue, ObservableCollection<FSAutomatorAction> actionList)
        {
            Trace.WriteLine("test lalalatest");
            finishEvent.Set();
            return "finish lalala.";
        }
    }
}
