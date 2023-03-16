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
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            var loggerActions = actionsList.Where(x => x.Name == "FlightPositionLogger").ToList();

            if (!loggerActions.Any())
            {
                return new ActionResult("No logger has been started", "No logger has been started", true);
            }

            var loggerAction = loggerActions.First();
            loggerAction.ActionObject.GetType().GetMethod("StopLogging").Invoke(this, new object[] { true });

            return new ActionResult("a", "a", false);
        }
    }
}
