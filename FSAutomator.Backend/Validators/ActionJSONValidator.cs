using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Utilities;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.BackEnd.Validators
{
    internal static class ActionJSONValidator
    {
        static internal List<string> ValidateActions(FSAutomatorAction[] actionList, string JSONFilePath)
        {
            actionList.ToList().ForEach(x => { x.IsValidated = false; x.ValidationOutcome = ""; });

            List<string> validationIssues = new List<string>();

            foreach (var (action, index) in actionList.Select((value, i) => (value, i)))
            {
                bool actionIsValidated = true;

                if (action.Name == "ConditionalAction")
                {
                    actionIsValidated = ValidateConditionalAction(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "ExecuteCodeFromDLL")
                {
                    actionIsValidated = ValidateExecuteCodeFromDLL(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "ExpectVariableValue")
                {
                    actionIsValidated = ValidateExpectVariableValue(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "GetVariable")
                {
                    actionIsValidated = ValidateGetVariable(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "MemoryRegisterRead")
                {
                    actionIsValidated = ValidateMemoryRegisterRead(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "MemoryRegisterWrite")
                {
                    actionIsValidated = ValidateMemoryRegisterWrite(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "OperateValue")
                {
                    actionIsValidated = ValidateOperateValue(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "SendEvent")
                {
                    actionIsValidated = ValidateSendEvent(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "WaitSeconds")
                {
                    actionIsValidated = ValidateWaitSeconds(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "WaitUntilVariableReachesNumericValue")
                {
                    actionIsValidated = ValidateWaitUntilVariableReachesNumericValue(actionList, validationIssues, index, action, JSONFilePath);
                }
                else if (action.Name == "DLLAutomation")
                {
                    actionIsValidated = ValidateDLLAutomation(actionList, validationIssues, index, action, JSONFilePath);
                }



                action.IsValidated = actionIsValidated;
            }

            return validationIssues;
        }

        private static bool ValidateConditionalAction(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            List<string> AllowedNumberComparisonValues = new List<string>() { "<", ">", "=", "<>" };
            List<string> AllowedStringComparisonValues = new List<string>() { "=", "<>" };

            var actionObject = (ConditionalAction)action.ActionObject;


            if ((!Utils.IsNumericDouble(actionObject.FirstMember)) || (!Utils.IsNumericDouble(actionObject.SecondMember)))
            {
                if (!AllowedStringComparisonValues.Contains(actionObject.Comparison))
                {
                    var issue = String.Format("ConditionalAction [{0}]: Comparing 2 strings ({1}, {2}) not supported by operator {3}.", index, actionObject.FirstMember, actionObject.SecondMember, actionObject.Comparison);
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);

                }
            }
            if (!AllowedNumberComparisonValues.Contains(actionObject.Comparison))
            {
                var issue = String.Format("ConditionalAction [{0}]: Comparing 2 numbers ({1}, {2}) not supported by operator {3}.", index, actionObject.FirstMember, actionObject.SecondMember, actionObject.Comparison);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }
            if (string.IsNullOrEmpty(actionObject.ActionIfTrueUniqueID) && string.IsNullOrEmpty(actionObject.ActionIfFalseUniqueID))
            {
                var issue = "Both true and false UniqueID for execution are missing";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);

                
            }

            return actionIsValidated;
        }

        private static bool ValidateExpectVariableValue(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;


            var variableName = (action.ActionObject as ExpectVariableValue).VariableName;

            if (!VariableExists(variableName))
            {
                var issue = String.Format("GetVariable [{0}]: Variable {1} does not exist.", index, variableName);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);

            }

            return actionIsValidated;
        }

        private static bool ValidateSendEvent(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            var eventName = (action.ActionObject as SendEvent).EventName;

            var eventExists = Enum.IsDefined(typeof(EVENTS), eventName);

            if (!eventExists)
            {
                var issue = String.Format("SendEvent [{0}]: Event {1} does not exist.", index, eventName);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateGetVariable(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            var variableName = (action.ActionObject as GetVariable).VariableName;
            
            if (!VariableExists(variableName))
            {
                var issue = String.Format("GetVariable [{0}]: Variable {1} does not exist.", index, variableName);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateWaitSeconds(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            var configuredWaitTime = (action.ActionObject as WaitSeconds).WaitTime;

            if (!(configuredWaitTime.Trim().Length > 0) || !Utils.IsNumericDouble(configuredWaitTime))
            {
                var issue = String.Format("WaitSeconds [{0}]: Specified WaitTime '{1}' value not valid.", index, configuredWaitTime);

                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateExecuteCodeFromDLL(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string JSONFilePath)
        {
            bool actionIsValidated = true;

            string JSONDirFullFilePath = Directory.GetParent(JSONFilePath).FullName;
            //"C:\\Users\\Albert\\source\\repos\\FSAutomator\\FSAutomator.UI\\bin\\Debug\\net6.0-windows\\Automations\\bb"

            var DLLPath = (action.ActionObject as ExecuteCodeFromDLL).DLLName;
            //"bbdll\\ExternalAutomationExample.dll"

            var realDLLPath = Path.Combine(JSONDirFullFilePath, DLLPath);

            if (DLLPath == String.Empty || !File.Exists(realDLLPath))
                {
                var issue = String.Format("DLLAutomation [{0}]: Referenced DLL ({1}) does not exist", index, JSONFilePath);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;

        }
        
        private static bool ValidateDLLAutomation(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string JSONFilePath)
        {
            bool actionIsValidated = true;

            string JSONDirFullFilePath = Directory.GetParent(JSONFilePath).FullName;
            //"C:\\Users\\Albert\\source\\repos\\FSAutomator\\FSAutomator.UI\\bin\\Debug\\net6.0-windows\\Automations"

            var DLLPath = (action.ActionObject as ExternalAutomator).DLLPath;
            //"Automations\\ExternalAutomationExample.dll"


            if (DLLPath == String.Empty || !File.Exists(DLLPath))
                {
                var issue = String.Format("DLLAutomation [{0}]: Referenced DLL ({1}) does not exist", index, JSONFilePath);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;

        }

        private static bool ValidateMemoryRegisterWrite(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            var actionID = (action.ActionObject as MemoryRegisterWrite).Id;
            var writeRegisterActionsUpToNow = actionList.TakeWhile(x => x != action).Where(y => y.Name == "MemoryRegisterWrite").ToList();
            var listActionsUsingSameWriteID = writeRegisterActionsUpToNow.Where(x => (x.ActionObject as MemoryRegisterWrite).Id == actionID).Any();

            if (listActionsUsingSameWriteID)
            {
                var issue = String.Format("MemoryRegisterWrite [{0}]: Id already used by another MemoryRegisterWrite operation.", index);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;

        }

        private static bool ValidateOperateValue(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            if (index == 0)
            {
                var issue = String.Format("OperateValue [{0}]: trying to operate on previous value but this is the first action in the automation.", index);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }
            else if (actionList[index - 1].Name == "GetVariable")
            {
                var variableName = (actionList[index - 1].ActionObject as GetVariable).VariableName;
                var variableType = new CommonEntities().DefineIDs[new Variable().GetVariableInformation(variableName).Type];
                var operation = (actionList[index].ActionObject as OperateValue).Operation;

                if ((variableType != CommonEntities.DEFINITIONS.NumType) && (variableType != CommonEntities.DEFINITIONS.BoolType))
                {
                    var issue = String.Format("OperateValue [{0}]: trying to operate on previous value but previous GetValue does neither return a numeric value nor a boolean value.", index); action.ValidationOutcome = issue;
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
                else if ((variableType == CommonEntities.DEFINITIONS.NumType) && operation == "NOT")
                {
                    var issue = String.Format("OperateValue [{0}]: trying to operate on numeric value using an operator for boolean values.", index);
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);

                }
                else if ((variableType == CommonEntities.DEFINITIONS.BoolType) && operation != "NOT")
                {
                    var issue = String.Format("OperateValue [{0}]: tryig to operate on boolean value using an operator for numeric values.", index);
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
            }

            return actionIsValidated;
        }

        private static bool ValidateWaitUntilVariableReachesNumericValue(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;

            var actionObject = action.ActionObject as WaitUntilVariableReachesNumericValue;
            var variableName = actionObject.VariableName;
            var variableType = new CommonEntities().DefineIDs[new Variable().GetVariableInformation(variableName).Type];

            if (!(actionObject.AllowedComparisonValues.Contains(actionObject.Comparison)))
            {
                var issue = String.Format("WaitUntilVariableReachesNumericValue [{0}]: Comparison {1} is not supported ", index, actionObject.Comparison);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            if (variableType != CommonEntities.DEFINITIONS.NumType)
            {
                var issue = String.Format("WaitUntilVariableReachesNumericValue [{0}]: trying monitor a value which is not numeric. Monitored varaible type is {1} ", index, variableType.ToString());
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateMemoryRegisterRead(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action, string jSONFilePath)
        {
            bool actionIsValidated = true;
            
            //The ID you are trying to read has not been written yet

            var idToRead = (action.ActionObject as MemoryRegisterRead).Id;

            var writeRegisterActionsUpToNow = actionList.TakeWhile(x => x != action).Where(y => y.Name == "MemoryRegisterWrite").ToList();
            var writtenIDsUpToNow = writeRegisterActionsUpToNow.Select(x => (x.ActionObject as MemoryRegisterWrite).Id).ToList();

            if (!writtenIDsUpToNow.Any())
            {
                var issue = String.Format("MemoryRegisterRead [{0}]: trying to read a register but any register has previously been written.", index);
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }
            else
            {
                var readRegisterActionsUpToNow = actionList.TakeWhile(x => x != action).Where(y => y.Name == "MemoryRegisterRead").ToList();
                var readIDsUpToNowWithSelectedRemoval = readRegisterActionsUpToNow.Where(z => (z.ActionObject as MemoryRegisterRead).RemoveAfterRead == true).Select(x => (x.ActionObject as MemoryRegisterRead).Id).ToList();

                var remainingIDsToRead = writtenIDsUpToNow.Except(readIDsUpToNowWithSelectedRemoval).ToList();

                if (!remainingIDsToRead.Contains(idToRead)){
                    var issue = String.Format("MemoryRegisterRead [{0}]: trying to read a register with ID not available. Maybe removed during previous read?", index);
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
            }

            return actionIsValidated;
        }

        private static bool VariableExists(string variableName)
        {
            var exists = true;

            var variableInformation = new Variable().GetVariableInformation(variableName);

            if ((variableInformation == null) || (variableInformation.Type == null))
            {
                return false;
            }

            return exists;
        }

        private static bool SetAsValidationFailed(List<string> validationIssues, string issue, FSAutomatorAction action)
        {
            action.ValidationOutcome = issue;
            validationIssues.Add(issue);
            return false;
        }
    }
}
