using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.BackEnd.Entities;
using Geolocation;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

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
