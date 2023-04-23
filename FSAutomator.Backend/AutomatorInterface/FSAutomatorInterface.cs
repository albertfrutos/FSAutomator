using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.AutomatorInterface.Managers;
using FSAutomator.SimConnectInterface;

namespace FSAutomator.Backend.AutomatorInterface
{
    public class FSAutomatorInterface : FSAutomatorInterfaceBaseActions
    {


        public GeneralStatus Status;

        public AutoPilotManager AutoPilotManager;
        public AdvancedActionsManager AdvancedActionsManager;

        internal AutoResetEvent FinishEvent = new AutoResetEvent(false);

        public FSAutomatorInterface(Automator automator, ISimConnectBridge connection, AutoResetEvent finishEvent) : base(automator, connection)
        {
            Status = GeneralStatus.GetInstance;
            FinishEvent = finishEvent;
            Connection = connection;

            AutoPilotManager = new AutoPilotManager(automator, connection);
            AdvancedActionsManager = new AdvancedActionsManager(automator, connection);
        }

        public void AutomationHasEnded()
        {
            FinishEvent.Set();
            return;
        }





    }
}