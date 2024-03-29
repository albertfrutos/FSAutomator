﻿using FSAutomator.Backend.Actions.Base;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.SimConnectInterface;
using System.Collections.ObjectModel;

namespace FSAutomator.Backend.Actions
{
    public class ConditionalAction : ActionBase, IAction
    {

        public string FirstMember { get; set; }
        public string Comparison { get; set; }
        public string SecondMember { get; set; }
        public string ActionIfTrueUniqueID { get; set; } = null;
        public string ActionIfFalseUniqueID { get; set; } = null;

        internal List<string> AllowedNumberComparisonValues = new List<string>() { "<", ">", "=", "<>" };
        internal List<string> AllowedStringComparisonValues = new List<string>() { "=", "<>" };

        internal FSAutomatorAction CurrentAction = null;

        public ConditionalAction() : base()
        {

        }

        public ConditionalAction(string firstMember, string comparison, string secondMember, string actionIfTrueUniqueID, string actionIfFalseUniqueID)
        {
            FirstMember = firstMember;
            Comparison = comparison;
            SecondMember = secondMember;
            ActionIfTrueUniqueID = actionIfTrueUniqueID;
            ActionIfFalseUniqueID = actionIfFalseUniqueID;
        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            bool isConditionTrue = false;

            ActionResult result = null;

            var actionsList = (sender as Automator).ActionList;
            this.CurrentAction = (FSAutomatorAction)actionsList.Where(x => x.Status == FSAutomatorAction.ActionStatus.Running).First();

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

            ObservableCollection<FSAutomatorAction> auxiliaryActionList = (sender as Automator).AuxiliaryActionList;

            if ((string.IsNullOrEmpty(ActionIfTrueUniqueID)) && (string.IsNullOrEmpty(ActionIfFalseUniqueID)))
            {
                return new ActionResult("Both true and false UniqueID for execution are missing", null, true);
            }
            else if (isConditionTrue && !string.IsNullOrEmpty(ActionIfTrueUniqueID))
            {
                result = ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfTrueUniqueID);
            }
            else if (!isConditionTrue && !string.IsNullOrEmpty(ActionIfFalseUniqueID))
            {
                result = ExecuteConditionalAction(sender, connection, auxiliaryActionList, ActionIfFalseUniqueID);
            }
            else
            {
                return new ActionResult(String.Format("Comparison is {0} but the UniqueID associated is null or empty", isConditionTrue.ToString()), null, true);

            }

            return new ActionResult($"{result.VisibleResult} - {isConditionTrue}", result.ComputedResult, result.Error);
        }

        private static ActionResult ExecuteConditionalAction(object sender, ISimConnectBridge connection, ObservableCollection<FSAutomatorAction> auxiliaryActionList, string actionUniqueID)
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
