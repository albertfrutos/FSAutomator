using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;

namespace FSAutomator.Backend.Actions
{
    public class ConditionalAction : IAction
    {

        public string FirstMember { get; set; }
        public string Comparison { get; set; }
        public string SecondMember { get; set; }
        public string ActionIfTrueUniqueID { get; set; } = null;
        public string ActionIfFalseUniqueID { get; set; } = null;

        internal List<string> AllowedNumberComparisonValues = new List<string>() { "<", ">", "=", "<>" };
        internal List<string> AllowedStringComparisonValues = new List<string>() { "=", "<>" };

        internal FSAutomatorAction CurrentAction = null;

        public ConditionalAction()
        {

        }

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
                // if one of the two members is not a number --> it can still be compared as a string

                if (AllowedStringComparisonValues.Contains(this.Comparison))
                {
                    // only '=' or '<>' comparisons are valid with strings

                    isConditionTrue = CheckCondition(this.FirstMember, this.SecondMember);
                }
                else
                {
                    return new ActionResult("String comparison only allowed with = or <>", null, true);
                }
            }
            else
            {
                // both members are a number

                isConditionTrue = CheckCondition(Convert.ToDouble(this.FirstMember), Convert.ToDouble(this.SecondMember));
            }

            ObservableCollection<FSAutomatorAction> auxiliaryActionList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("AuxiliaryActionList").GetValue(sender);

            if ((string.IsNullOrEmpty(ActionIfTrueUniqueID)) && (string.IsNullOrEmpty(ActionIfFalseUniqueID)))
            {
                return new ActionResult("Both true and false UniqueID for execution are missing", null, true);
            }
            else if (isConditionTrue && !string.IsNullOrEmpty(ActionIfTrueUniqueID))
            {
                result = ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfTrueUniqueID);
            }
            else if (!string.IsNullOrEmpty(ActionIfFalseUniqueID))
            {
                result = ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfFalseUniqueID);
            }

            return new ActionResult($"{result.VisibleResult} - {isConditionTrue}", result.ComputedResult);
        }

        private static ActionResult ExecuteConditionalAction(object sender, SimConnect connection, ObservableCollection<FSAutomatorAction> auxiliaryActionList, string actionUniqueID)
        {
            var action = auxiliaryActionList.Where(x => x.UniqueID == actionUniqueID).First();
            ActionResult result = (ActionResult)action.ActionObject.GetType().GetMethod("ExecuteAction").Invoke(action.ActionObject, new object[] { sender, connection });
            return result;
        }

        private bool CheckCondition(dynamic firstMember, dynamic secondMember)
        {
            var result = Comparison switch
            {
                "<" => firstMember < secondMember,
                ">" => firstMember > secondMember,
                "=" or "==" => firstMember == secondMember,
                "<>" => firstMember != secondMember,
                _ => false,
            };
            return result;
        }
    }
}
