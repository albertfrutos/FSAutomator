using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Newtonsoft.Json;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.BackEnd.Validators
{
    internal static class ActionJSONValidator
    {
        static internal List<string> ValidateActions(FSAutomatorAction[] actionList)
        {
            actionList.ToList().ForEach(x => { x.IsValidated = false; x.ValidationOutcome = ""; });

            List<string> validationIssues = new List<string>();

            foreach (var (action, index) in actionList.Select((value, i) => (value, i)))
            {
                bool actionIsValidated = true;

                var jsonValidationResult = ValidateJSON(action, validationIssues, index, Type.GetType($"FSAutomator.Backend.Actions.{action.Name}"));

                if (jsonValidationResult && action.ActionObject != null)
                {
                    switch (action.Name)
                    {
                        case "ConditionalAction":
                            actionIsValidated = ValidateConditionalAction(actionList, validationIssues, index, action);
                            break;
                        case "ExecuteCodeFromDLL":
                            actionIsValidated = ValidateExecuteCodeFromDLL(actionList, validationIssues, index, action);
                            break;
                        case "ExpectVariableValue":
                            actionIsValidated = ValidateExpectVariableValue(actionList, validationIssues, index, action);
                            break;
                        case "GetVariable":
                            actionIsValidated = ValidateGetVariable(actionList, validationIssues, index, action);
                            break;
                        case "MemoryRegisterRead":
                            actionIsValidated = ValidateMemoryRegisterRead(actionList, validationIssues, index, action);
                            break;
                        case "MemoryRegisterWrite":
                            actionIsValidated = ValidateMemoryRegisterWrite(actionList, validationIssues, index, action);
                            break;
                        case "OperateValue":
                            actionIsValidated = ValidateOperateValue(actionList, validationIssues, index, action);
                            break;
                        case "SendEvent":
                            actionIsValidated = ValidateSendEvent(actionList, validationIssues, index, action);
                            break;
                        case "WaitSeconds":
                            actionIsValidated = ValidateWaitSeconds(actionList, validationIssues, index, action);
                            break;
                        case "WaitUntilVariableReachesNumericValue":
                            actionIsValidated = ValidateWaitUntilVariableReachesNumericValue(actionList, validationIssues, index, action);
                            break;
                        case "DLLAutomation":
                            actionIsValidated = ValidateDLLAutomation(actionList, validationIssues, index, action);
                            break;
                        case "FlightPositionLogger":
                            actionIsValidated = ValidateFlightPositionLogger(actionList, validationIssues, index, action);
                            break;
                        case "FlightPositionLoggerStop":
                            actionIsValidated = ValidateFlightPositionLoggerStop(actionList, validationIssues, index, action);
                            break;
                    }
                }

                action.IsValidated = actionIsValidated && jsonValidationResult;
            }

            return validationIssues;
        }



        private static bool ValidateJSON(FSAutomatorAction action, List<string> validationIssues, int index, Type actionType)
        {
            var actionObject = action.Parameters;

            try
            {   
                JsonConvert.DeserializeObject(actionObject, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                var issue = $"{action.Name} [{index}]: ActionObject contains an incorrect (malformed) JSON. The action cannot be validated until the JSON is valid - {errorMessage}";
                return SetAsValidationFailed(validationIssues, issue, action);
            }

            return true;
        }

        private static bool ValidateFlightPositionLogger(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var actionObject = (FlightPositionLogger)action.ActionObject;


           if (actionObject.LoggingPeriodSeconds <= 0)
            {
                var issue = $"FlightPositionLogger [{index}]: LoggingPeriodSeconds must be higher or equal to 0.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            if (actionObject.LoggingTimeSeconds == 0)
            {
                var existsFlightPositionLoggerActionAfterStartLogging = actionList.Reverse().TakeWhile(x => x != action).Where(y => y.Name == "FlightPositionLoggerStop").Any();

                if (!actionObject.LogInNoLockingBackgroundMode)
                {
                    var issue = $"FlightPositionLogger [{index}]: The logger is configured to never end and not running in background mode. The logging will never end and will be never written to disk.";
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }

                if (actionObject.LogInNoLockingBackgroundMode && !existsFlightPositionLoggerActionAfterStartLogging)
                {
                    var issue = $"FlightPositionLogger [{index}]: There is no FlightPositionLoggerStop action after FlightPositionLogger with no-ending time. Logging will never end and will never be written to disk.";
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
            }

            return actionIsValidated;
        }

        private static bool ValidateFlightPositionLoggerStop(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var existsFlightPositionLoggingActionBeforeStopLogging = actionList.TakeWhile(x => x != action).Where(y => y.Name == "FlightPositionLogger").Any();

            if (!existsFlightPositionLoggingActionBeforeStopLogging)
            {
                var issue = $"FlightPositionLoggerStop [{index}]: There is no FlightPositionLogger action before FlightPositionLoggerStop (there is nothing to stop).";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateConditionalAction(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            List<string> AllowedNumberComparisonValues = new List<string>() { "<", ">", "=", "<>" };
            List<string> AllowedStringComparisonValues = new List<string>() { "=", "<>" };

            var actionObject = (ConditionalAction)action.ActionObject;

            if (!AllowedNumberComparisonValues.Concat(AllowedStringComparisonValues).Contains(actionObject.Comparison))
            {
                var issue = $"ConditionalAction [{index}]: The comparison operator ({actionObject.Comparison} is not supported by operator.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                return actionIsValidated;
            }
           

            if ((!Utils.IsNumericDouble(actionObject.FirstMember)) || (!Utils.IsNumericDouble(actionObject.SecondMember)))
            {
                if (!AllowedStringComparisonValues.Contains(actionObject.Comparison))
                {
                    var issue = $"ConditionalAction [{index}]: Comparing 2 strings ({actionObject.FirstMember}, {actionObject.SecondMember}) not supported by operator {actionObject.Comparison}.";
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
            }

            if (!AllowedNumberComparisonValues.Contains(actionObject.Comparison))
            {
                var issue = $"ConditionalAction [{index}]: Comparing 2 numbers ({actionObject.FirstMember}, {actionObject.SecondMember}) not supported by operator {actionObject.Comparison}.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            if (string.IsNullOrEmpty(actionObject.ActionIfTrueUniqueID) && string.IsNullOrEmpty(actionObject.ActionIfFalseUniqueID))
            {
                var issue = $"ConditionalAction[{index}]: Both true and false UniqueID for execution are missing";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateExpectVariableValue(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;


            var variableName = (action.ActionObject as ExpectVariableValue).VariableName;

            if (!VariableExists(variableName))
            {
                var issue = $"GetVariable [{index}]: Variable {variableName} does not exist.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateSendEvent(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var eventName = (action.ActionObject as SendEvent).EventName;

            var eventExists = Enum.IsDefined(typeof(EVENTS), eventName);

            if (!eventExists)
            {
                var issue = $"SendEvent [{index}]: Event {eventName} does not exist.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateGetVariable(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var variableName = (action.ActionObject as GetVariable).VariableName;

            if (!VariableExists(variableName))
            {
                var issue = $"GetVariable [{index}]: Variable {variableName} does not exist.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateWaitSeconds(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            
            var configuredWaitTime = (action.ActionObject as WaitSeconds).WaitTime;

            if (!(configuredWaitTime > 0))
            {
                var issue = $"WaitSeconds [{index}]: Specified WaitTime '{configuredWaitTime}' value not valid. Must be grater than 0.";

                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }
            

            return actionIsValidated;
        }

        private static bool ValidateExecuteCodeFromDLL(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {

            bool actionIsValidated = true;

            var DLLName = (action.ActionObject as ExecuteCodeFromDLL).DLLName;

            var realDLLPath = Path.Combine(action.AutomationFile.BasePath, DLLName);

            if (DLLName == String.Empty || !File.Exists(realDLLPath))
            {
                var issue = $"DLLAutomation [{index}]: Referenced DLL ({realDLLPath}) does not exist";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;

        }

        private static bool ValidateDLLAutomation(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var DLLPath = action.AutomationFile.FilePath;

            if (DLLPath == String.Empty || !File.Exists(DLLPath))
            {
                var issue = $"DLLAutomation [{index}]: Referenced DLL ({DLLPath}) does not exist";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;

        }

        private static bool ValidateMemoryRegisterWrite(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var actionID = (action.ActionObject as MemoryRegisterWrite).Id;
            var writeRegisterActionsUpToNow = actionList.TakeWhile(x => x != action).Where(y => y.Name == "MemoryRegisterWrite").ToList();
            var listActionsUsingSameWriteID = writeRegisterActionsUpToNow.Where(x => (x.ActionObject as MemoryRegisterWrite).Id == actionID).Any();

            if (listActionsUsingSameWriteID)
            {
                var issue = $"MemoryRegisterWrite [{index}]: Id already used by another MemoryRegisterWrite operation.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;

        }

        private static bool ValidateOperateValue(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            if (index == 0)
            {
                var issue = $"OperateValue [{index}]: trying to operate on previous value but this is the first action in the automation.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }
            else if (actionList[index - 1].Name == "GetVariable")
            {
                var variableName = (actionList[index - 1].ActionObject as GetVariable).VariableName;
                var variableType = new CommonEntities().DefineIDs[new Variable().GetVariableInformation(variableName).Type];
                var operation = (actionList[index].ActionObject as OperateValue).Operation;

                if ((variableType != CommonEntities.DEFINITIONS.NumType) && (variableType != CommonEntities.DEFINITIONS.BoolType))
                {
                    var issue = $"OperateValue [{index}]: trying to operate on previous value but previous GetValue does neither return a numeric value nor a boolean value."; 
                    action.ValidationOutcome = issue;
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
                else if ((variableType == CommonEntities.DEFINITIONS.NumType) && operation == "NOT")
                {
                    var issue = $"OperateValue [{index}]: trying to operate on numeric value using an operator for boolean values.";
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);

                }
                else if ((variableType == CommonEntities.DEFINITIONS.BoolType) && operation != "NOT")
                {
                    var issue = $"OperateValue [{index}]: tryig to operate on boolean value using an operator for numeric values.";
                    actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
                }
            }

            return actionIsValidated;
        }

        private static bool ValidateWaitUntilVariableReachesNumericValue(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            var actionObject = action.ActionObject as WaitUntilVariableReachesNumericValue;
            var variableName = actionObject.VariableName;
            var variableType = new CommonEntities().DefineIDs[new Variable().GetVariableInformation(variableName).Type];

            if (!(actionObject.AllowedComparisonValues.Contains(actionObject.Comparison)))
            {
                var issue = $"WaitUntilVariableReachesNumericValue [{index}]: Comparison {actionObject.Comparison} is not supported ";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            if (variableType != CommonEntities.DEFINITIONS.NumType)
            {
                var issue = $"WaitUntilVariableReachesNumericValue [{index}]: trying monitor a value which is not numeric. Monitored varaible type is {variableType} ";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }

            return actionIsValidated;
        }

        private static bool ValidateMemoryRegisterRead(FSAutomatorAction[] actionList, List<string> validationIssues, int index, FSAutomatorAction action)
        {
            bool actionIsValidated = true;

            //The ID you are trying to read has not been written yet

            var idToRead = (action.ActionObject as MemoryRegisterRead).Id;

            var writeRegisterActionsUpToNow = actionList.TakeWhile(x => x != action).Where(y => y.Name == "MemoryRegisterWrite").ToList();
            var writtenIDsUpToNow = writeRegisterActionsUpToNow.Select(x => (x.ActionObject as MemoryRegisterWrite).Id).ToList();

            if (!writtenIDsUpToNow.Any())
            {
                var issue = $"MemoryRegisterRead [{index}]: trying to read a register but any register has previously been written.";
                actionIsValidated = SetAsValidationFailed(validationIssues, issue, action);
            }
            else
            {
                var readRegisterActionsUpToNow = actionList.TakeWhile(x => x != action).Where(y => y.Name == "MemoryRegisterRead").ToList();
                var readIDsUpToNowWithSelectedRemoval = readRegisterActionsUpToNow.Where(z => (z.ActionObject as MemoryRegisterRead).RemoveAfterRead == true).Select(x => (x.ActionObject as MemoryRegisterRead).Id).ToList();

                var remainingIDsToRead = writtenIDsUpToNow.Except(readIDsUpToNowWithSelectedRemoval).ToList();

                if (!remainingIDsToRead.Contains(idToRead))
                {
                    var issue = $"MemoryRegisterRead [{index}]: trying to read a register with ID not available. Maybe removed during previous read?";
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
