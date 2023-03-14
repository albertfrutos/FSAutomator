using FSAutomator.Backend.Actions;
using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.BackEnd.AutomatorInterface.Managers
{
    public class AdvancedActionsManager : FSAutomatorInterfaceBaseActions
    {
        private EventHandler<bool> finishFlightPositionLoggerEvent = null;

        public AdvancedActionsManager(Automator automator, SimConnect connection) : base(automator, connection)
        {

        }
        public string CalculateBearingToCoordinates(string latitude, string longitude)
        {
            var result = new CalculateBearingToCoordinates(latitude, longitude).ExecuteAction(this, Connection).ComputedResult;
            return result;
        }

        public string CalculateDistanceToCoordinates(string latitude, string longitude)
        {
            var result = new CalculateDistanceToCoordinates(latitude, longitude).ExecuteAction(this, Connection).ComputedResult;
            return result;
        }
        
        public ActionResult FlightPositionLogger(string loggingTimeSeconds, string loggingPeriodSeconds, string logInNoLockingBackgroundMode = "false")
        {
            var result = new FlightPositionLogger(loggingTimeSeconds, loggingPeriodSeconds, logInNoLockingBackgroundMode).ExecuteAction(this, Connection);
            
            if(logInNoLockingBackgroundMode == "true")
            {
                finishFlightPositionLoggerEvent = result.ReturnObject;
                return result;
            }

            return new ActionResult("Logger finished", "Logger finished", false);
        }

        public ActionResult FlightPositionLoggerStop(bool isManualStop = false)
        {
            if(finishFlightPositionLoggerEvent is null)
            {
                return new ActionResult("Logger has not been started", "Logger has not been started", true);
            }

            finishFlightPositionLoggerEvent.Invoke(this, true);
            finishFlightPositionLoggerEvent = null;
            return new ActionResult("Logger finished", "Logger finished", false);
        }


    }
}
