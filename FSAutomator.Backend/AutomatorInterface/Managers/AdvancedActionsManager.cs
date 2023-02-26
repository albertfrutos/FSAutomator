using FSAutomator.Backend.Actions;
using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Automators;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.BackEnd.AutomatorInterface.Managers
{
    public class AdvancedActionsManager : FSAutomatorInterfaceBaseActions
    {
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


    }
}
