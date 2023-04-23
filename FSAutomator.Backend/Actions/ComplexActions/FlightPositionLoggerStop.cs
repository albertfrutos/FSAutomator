using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Actions
{
    public class FlightPositionLoggerStop
    {
        public FlightPositionLoggerStop()
        {

        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var actionsList = (sender as Automator).ActionList;
            var loggerActions = actionsList.Where(x => x.Name == "FlightPositionLogger").ToList();

            if (!loggerActions.Any())
            {
                return new ActionResult("No logger has been started", "No logger has been started", true);
            }

            var loggerAction = loggerActions.First();
            (loggerAction.ActionObject as FlightPositionLogger).StopLogging(this, true);

            return new ActionResult("Logger stopped", "Logger Stopped", false);
        }
    }
}
