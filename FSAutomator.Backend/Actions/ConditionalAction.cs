using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Entities;
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


        internal string[] AllowedNumberComparisonValues = { "<", ">", "=", "==" };
        internal string[] AllowedStringComparisonValues = { "=", "==" };

        internal FlightModel fm;

        private string variableValue = string.Empty;

        internal FSAutomatorAction? CurrentAction = null;

        internal string actionResult = "";


        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            bool isConditionTrue = false;

            ActionResult result = null;

            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            this.CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == "Running").First();

            this.FirstMember = Utils.GetValueToOperateOnFromTag(sender, connection, this.FirstMember);
            this.SecondMember = Utils.GetValueToOperateOnFromTag(sender, connection, this.SecondMember);

            if ((!Utils.IsNumericDouble(this.FirstMember)) || (!Utils.IsNumericDouble(this.SecondMember)))
            {
                // if one of the two members is not a number --> ir can still be compared as a string

                if (AllowedStringComparisonValues.Contains(this.Comparison))
                {
                    // only '=' and '==' comparisons are valid with strings

                    isConditionTrue = CheckCondition(this.FirstMember, this.SecondMember);
                }
                else
                {
                    return new ActionResult("String comparison only allowed with = or ==.", null);
                }
                
            }
            else
            {
                // both members are a number;

                isConditionTrue = CheckCondition(Convert.ToDouble(this.FirstMember), Convert.ToDouble(this.SecondMember));
            }

            ObservableCollection<FSAutomatorAction> auxiliaryActionList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("AuxiliaryActionList").GetValue(sender);

            //note: if actiontrueuniqueid or actionfalseuniqueid are null, return and send warning via event
            //note: compare strings, pending to implement

            

            if (isConditionTrue)
            {
                result = ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfTrueUniqueID);
            }
            else if (ActionIfFalseUniqueID != null)
            {
                result = ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfFalseUniqueID);
            }

            return new ActionResult($"{result.VisibleResult} - {isConditionTrue}", result.ComputedResult);

        }

        private ActionResult ExecuteConditionalAction(object sender, SimConnect connection, ObservableCollection<FSAutomatorAction> auxiliaryActionList, string actionUniqueID)
        {
            var action = auxiliaryActionList.Where(x => x.UniqueID == actionUniqueID).First();
            ActionResult result = (ActionResult)action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { sender, connection});
            return result;
        }

        private bool CheckCondition(dynamic firstMember, dynamic secondMember)
        {
            bool result = false;
            
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
