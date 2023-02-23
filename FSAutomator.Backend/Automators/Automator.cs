using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Automators
{
    public class Automator
    {
        public SimConnect connection;

        public Dictionary<string, string> MemoryRegisters = new Dictionary<string, string>();
        public FlightModel flightModel;

        public string lastOperationValue = "";

        public ObservableCollection<FSAutomatorAction> ActionList = new ObservableCollection<FSAutomatorAction>();
        public ObservableCollection<FSAutomatorAction> AuxiliaryActionList = new ObservableCollection<FSAutomatorAction>();

        public GeneralStatus status = GeneralStatus.GetInstance;

        public Automator()
        {

        }

        public void ExecuteActionList()
        {
            //status.ReportEvent("prova reportevent"); // activar
            //return;

            if(this.connection == null)
            {
                var message = new InternalMessage("Connection not active","Error",true); //handle
                status.ReportError(message);
                return;
            }

            this.flightModel = new FlightModel(this.connection);

            foreach (FSAutomatorAction action in ActionList)
            {

                var stopExecution = RunAndProcessAction(action);

                if (stopExecution)
                {
                    Trace.WriteLine("An error caused the automation to stop (as configured)");
                    break;
                    
                }
            }

        }

        private bool RunAndProcessAction(FSAutomatorAction action)
        {
            action.Status = "Running";
            ActionResult result = ExecuteAction(action);
            lastOperationValue = result.ComputedResult;
            action.Result = result;
            action.Status = "Done";

            var stopExecution = result.Error && action.StopOnError;

            return stopExecution;
        }

        internal ActionResult ExecuteAction(FSAutomatorAction action)
        {
            var result = (ActionResult)action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { this, connection });
            return result;
        }

        internal void RebuildActionListIndices()
        {
            ActionList.ToList().ForEach(x => { x.Id = ActionList.IndexOf(x); });
        }
    }
}
