using FSAutomator.Backend.Actions;
using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.AutomatorInterface.Managers
{
    public class AdvancedActionsManager : FSAutomatorInterfaceBaseActions
    {
        private EventHandler<bool> finishFlightPositionLoggerEvent = null;

        public AdvancedActionsManager(Automator automator, ISimConnectBridge connection) : base(automator, connection)
        {

        }
        public string CalculateBearingToCoordinates(double latitude, double longitude)
        {
            var result = new CalculateBearingToCoordinates(latitude, longitude, new GetVariable()).ExecuteAction(this, Connection).ComputedResult;
            return result;
        }

        public string CalculateDistanceToCoordinates(double latitude, double longitude)
        {
            var result = new CalculateDistanceToCoordinates(latitude, longitude, new GetVariable()).ExecuteAction(this, Connection).ComputedResult;
            return result;
        }

        public ActionResult FlightPositionLogger(int loggingTimeSeconds, int loggingPeriodSeconds, bool logInNoLockingBackgroundMode = false)
        {
            var result = new FlightPositionLogger(loggingTimeSeconds, loggingPeriodSeconds, new GetVariable(), logInNoLockingBackgroundMode).ExecuteAction(this, Connection);

            if (logInNoLockingBackgroundMode)
            {
                finishFlightPositionLoggerEvent = result.ReturnObject;
                return result;
            }

            return new ActionResult("Logger finished", "Logger finished", false);
        }

        public ActionResult FlightPositionLoggerStop(bool isManualStop = false)
        {
            if (finishFlightPositionLoggerEvent is null)
            {
                return new ActionResult("Logger has not been started", "Logger has not been started", true);
            }

            finishFlightPositionLoggerEvent.Invoke(this, true);
            finishFlightPositionLoggerEvent = null;
            return new ActionResult("Logger finished", "Logger finished", false);
        }


    }
}
