using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Actions
{
    public class ConditionalAction
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

        internal string actionResult = "";


        public string ExecuteAction(object sender, SimConnect connection)
        {

            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            this.CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

            this.FirstMember = Utils.GetValueToOperateOnFromTag(sender, connection, this.FirstMember);
            this.SecondMember = Utils.GetValueToOperateOnFromTag(sender, connection, this.SecondMember);

            if ((!Utils.IsNumericDouble(this.FirstMember)) && (!Utils.IsNumericDouble(this.SecondMember)))
            {
                return String.Format("At least one member of the condition is not a number - {0} - {1}", this.FirstMember, this.SecondMember);
            }

            ObservableCollection<FSAutomatorAction> auxiliaryActionList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("AuxiliaryActionList").GetValue(sender);

            //note: if actiontrueuniqueid or actionfalseuniqueid are null, return and send warning via event
            //note: compare strings, pending to implement
            if (CheckCondition())
            {
                ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfTrueUniqueID);
            }
            else if (ActionIfFalseUniqueID != null)
            {
                ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfFalseUniqueID);
            }

            return "finished";

        }

        private void ExecuteConditionalAction(object sender, SimConnect connection, ObservableCollection<FSAutomatorAction> auxiliaryActionList, string actionUniqueID)
        {
            var action = auxiliaryActionList.Where(x => x.UniqueID == actionUniqueID).First();
            action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { sender, connection});
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
