using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.AutomatorInterface.Managers;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.AutomatorInterface
{
    public class FSAutomatorInterface : FSAutomatorInterfaceBaseActions
    {


        public GeneralStatus Status;

        public AutoPilotManager APManager;
        public AdvancedActionsManager AAManager;

        internal AutoResetEvent FinishEvent = new AutoResetEvent(false);

        public FSAutomatorInterface(Automator automator, SimConnect connection, AutoResetEvent finishEvent) : base(automator, connection)
        {
            Status = GeneralStatus.GetInstance;
            FinishEvent = finishEvent;
            Connection = connection;

            APManager = new AutoPilotManager(automator, connection);
            AAManager = new AdvancedActionsManager(automator, connection);
        }

        public void AutomationHasEnded()
        {
            FinishEvent.Set();
            return;
        }





    }
}