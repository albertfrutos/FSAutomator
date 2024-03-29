﻿using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using System.Collections.ObjectModel;
using static FSAutomator.Backend.Entities.FSAutomatorAction;

namespace FSAutomator.Backend.Automators
{
    public class Automator
    {
        public ISimConnectBridge Connection;

        public Dictionary<string, string> MemoryRegisters = new Dictionary<string, string>();
        public FlightModel flightModel;

        public string lastOperationValue = "";

        public ObservableCollection<FSAutomatorAction> ActionList = new ObservableCollection<FSAutomatorAction>();
        public ObservableCollection<FSAutomatorAction> AuxiliaryActionList = new ObservableCollection<FSAutomatorAction>();

        public GeneralStatus status = GeneralStatus.GetInstance;

        public Automator()
        {

        }

        public Automator(ISimConnectBridge connection)
        {
            this.Connection = connection;
        }

        public void ExecuteActionList()
        {

            if (!this.status.IsConnectedToSim)
            {
                var message = new InternalMessage("Connection not active", true, false);
                status.ReportStatus(message);
                return;
            }


            this.flightModel = new FlightModel(this.Connection);

            var isThereAnyInvalidJSON = ActionList.Where(x => x.ActionObject == null).Any();

            if (isThereAnyInvalidJSON)
            {
                status.ReportStatus(new InternalMessage("There are some actions with an invalid JSON", true, true));
                return;
            }

            foreach (FSAutomatorAction action in ActionList)
            {
                var errorOccurred = false;

                if (action.ParallelLaunch)
                {
                    Task.Run(() => errorOccurred = ProcessAction(action));
                }
                else
                {
                    errorOccurred = ProcessAction(action);
                }

                if (errorOccurred)
                {
                    break;
                }
            }

            status.ReportStatus(new InternalMessage("Automation finished", false, false));

        }

        private bool ProcessAction(FSAutomatorAction action)
        {
            var stopExecutionByActionError = RunAction(action);

            if (stopExecutionByActionError)
            {
                var error = new InternalMessage()
                {
                    Message = $"An error caused the automation to stop (as configured in [{ActionList.IndexOf(action)}]{action.Name} - {action.UniqueID})",
                    Type = InternalMessage.MsgType.Error
                };

                status.ReportStatus(error);
            }

            if (status.GeneralErrorHasOcurred)
            {
                var error = new InternalMessage()
                {
                    Message = $"A general critical error caused the automation to stop ([{ActionList.IndexOf(action)}]{action.Name} - {action.UniqueID})",
                    Type = InternalMessage.MsgType.Error
                };

                status.ReportStatus(error);
            }

            return stopExecutionByActionError || status.GeneralErrorHasOcurred;
        }

        private bool RunAction(FSAutomatorAction action)
        {
            action.Status = ActionStatus.Running;

            ActionResult result = ExecuteAction(action);

            action.Status = ActionStatus.Done;
            lastOperationValue = result.ComputedResult;
            action.Result = result;

            var stopExecutionByActionError = result.Error && action.StopOnError;

            return stopExecutionByActionError;
        }

        internal ActionResult ExecuteAction(FSAutomatorAction action)
        {
            var result = (ActionResult)(action.ActionObject as dynamic).ExecuteAction(this, Connection);
            return result;
        }

        internal void RebuildActionListIndices()
        {
            ActionList.ToList().ForEach(x => { x.Id = ActionList.IndexOf(x); });
        }
    }
}
