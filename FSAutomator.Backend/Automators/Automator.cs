﻿using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Automators
{
    public class Automator
    {
        public SimConnect connection;

        public event EventHandler<string> NewReturnValue;
        public event EventHandler UnlockNextStep;

        public Dictionary<string, string> MemoryRegisters = new Dictionary<string, string>();
        public FlightModel flightModel;
        

        public string lastOperationValue = "";

        public ObservableCollection<FSAutomatorAction> ActionList;
        public ObservableCollection<FSAutomatorAction> AuxiliaryActionList;

        public Automator(ObservableCollection<FSAutomatorAction> ActionList)
        {
            this.ActionList = ActionList;
            this.AuxiliaryActionList = new ObservableCollection<FSAutomatorAction>();
        }

        public void Execute()
        {
            foreach (FSAutomatorAction action in ActionList)
            {
                var stopExecution = RunAction(action);

                if (stopExecution)
                {
                    Trace.WriteLine("STOPPING EXECUTION!");
                    break;
                    
                }
            }

        }

        private bool RunAction(FSAutomatorAction action)
        {
            //note propietat a result per marcar error (un bool) i no usar el computed com a error si es null, sinó com el que toca

            action.Status = "Running";
            ActionResult result = (ActionResult)action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { this, connection });
            lastOperationValue = result.ComputedResult;
            action.Result = result.VisibleResult;
            action.Status = "Done";

            var stopExecution = result.Error && action.StopOnError;

            return stopExecution;
        }
    }
}
