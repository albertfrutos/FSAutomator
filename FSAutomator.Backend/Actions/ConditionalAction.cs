using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Actions
{
    public class ConditionalAction : IAction
    {

        public string FirstMember { get; set; }
        public string Comparison { get; set; }
        public string SecondMember { get; set; }
        public string ActionIfTrueUniqueID { get; set; } = null;
        public string ActionIfFalseUniqueID { get; set; } = null;
        public bool IsAuxiliary { get; set; } = false;


        internal string[] AllowedComparisonValues = { "<", ">", "=" };

        internal FlightModel fm;

        private string variableValue = string.Empty;

        internal FSAutomatorAction? CurrentAction = null;

        AutoResetEvent ContinueAfterAction = new AutoResetEvent(false);

        public event EventHandler Unlock;
        public event EventHandler<string> ReportActionResult;

        internal string actionResult = "";


        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            //ReportActionResult += GetActionResult;
            Unlock += UnlockAction;

            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            this.CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

            this.FirstMember = Utils.GetValueToOperateOnFromTag(sender, connection, this.FirstMember);
            this.SecondMember = Utils.GetValueToOperateOnFromTag(sender, connection, this.SecondMember);

            if ((!Utils.IsNumericDouble(this.FirstMember)) && (!Utils.IsNumericDouble(this.SecondMember)))
            {
                ReturnValueEvent.Invoke(this, String.Format("At least one member of the condition is not a number - {0} - {1}", this.FirstMember, this.SecondMember));
                UnlockNextStep.Invoke(this, null);
                return;
            }

            ObservableCollection<FSAutomatorAction> auxiliaryActionList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("AuxiliaryActionList").GetValue(sender);

            //note: if actiontrueuniqueid or actionfalseuniqueid are null, return and send warning via event

            if (CheckCondition())
            {
                ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfTrueUniqueID);
            }
            else if (ActionIfFalseUniqueID != null)
            {
                ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfFalseUniqueID);
            }

            ContinueAfterAction.WaitOne();



            ReturnValueEvent.Invoke(this, String.Format("{0}", this.actionResult));
            UnlockNextStep.Invoke(this, null);

        }

        private void ExecuteConditionalAction(object sender, SimConnect connection, ObservableCollection<FSAutomatorAction> auxiliaryActionList, string actionUniqueID)
        {
            var action = auxiliaryActionList.Where(x => x.UniqueID == actionUniqueID).First();
            action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { sender, connection, ReportActionResult, Unlock });
        }

        private void UnlockAction(object? sender, EventArgs e)
        {
            ContinueAfterAction.Set();

        }

        private void GetActionResult(object? sender, string e)
        {
            actionResult = e;
            Trace.WriteLine("conditional    "+ e);
        }

        private bool CheckCondition()
        {
            bool result = false;
            
            var firstMember = Convert.ToDouble(this.FirstMember);
            var secondMember = Convert.ToDouble(this.SecondMember);

            switch (Comparison)
            {
                case "<":
                    result = firstMember < secondMember;
                    break;
                case ">":
                    result = firstMember > secondMember;
                    break;
                case "=":
                case "==":
                    result = firstMember == secondMember;
                    break;
                default:
                    result = false;
                    break;
            }
            
            return result;

        }


    }
}
