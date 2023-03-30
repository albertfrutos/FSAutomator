using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Utilities;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Entities
{
    public class FSAutomatorAction : INotifyPropertyChanged
    {
        private int i_Id;
        private string s_Name;
        private string s_UniqueID;
        private ActionStatus e_Status;
        private string s_Parameters;
        private ActionResult s_Result;
        private object o_Object;
        private bool b_isCurrent;
        private bool b_isValidated;
        private bool b_isAuxiliary;
        private bool b_stopOnError;
        private string s_validationOutcome;
        private string s_mainFilePath;
        private AutomationFile o_AutomationFile;

        public FSAutomatorAction(string name, string uniqueID, ActionStatus status, string parameters, bool isAuxiliary, bool stopOnError, AutomationFile automationFile)
        {


            dynamic actionObject = null;

            if (name == "DLLAutomation")
            {
                actionObject = new ExternalAutomator(automationFile.FileName, parameters);
            }
            else
            {
                Type actionType = Type.GetType(String.Format("FSAutomator.Backend.Actions.{0}", name));
                //var actionObject = Activator.CreateInstance(actionType);

                dynamic dParameters = parameters == null ? "" : JsonConvert.DeserializeObject(parameters, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

                switch (actionType.Name)
                {
                    case "ConditionalAction":
                        actionObject = new ConditionalAction(dParameters.FirstMember, dParameters.Comparison, dParameters.SecondMember, dParameters.ActionIfTrueUniqueID, dParameters.ActionIfFalseUniqueID);
                        break;
                    case "ExecuteCodeFromDLL":
                        actionObject = new ExecuteCodeFromDLL(dParameters.DLLName, dParameters.DLLPath, dParameters.ClassName, dParameters.MethodName, dParameters.IncludeAsExternalAutomator);
                        actionObject.PackFolder = automationFile.PackageName;
                        break;
                    case "ExpectVariableValue":
                        actionObject = new ExpectVariableValue(dParameters.VariableName, dParameters.VariableExpectedValue, new GetVariable());
                        break;
                    case "GetVariable":
                        actionObject = new GetVariable(dParameters.VariableName);
                        break;
                    case "MemoryRegisterRead":
                        actionObject = new MemoryRegisterRead(Convert.ToBoolean(dParameters.RemoveAfterRead), dParameters.Id);
                        break;
                    case "MemoryRegisterWrite":
                        actionObject = new MemoryRegisterWrite(dParameters.Value, dParameters.Id);
                        break;
                    case "OperateValue":
                        actionObject = new OperateValue(dParameters.Operation, dParameters.Number, dParameters.ItemToOperateOver);
                        break;
                    case "SendEvent":
                        actionObject = new SendEvent(dParameters.EventName, dParameters.EventValue);
                        break;
                    case "WaitSeconds":
                        actionObject = new WaitSeconds(Convert.ToInt32(dParameters.WaitTime));
                        break;
                    case "WaitUntilVariableReachesNumericValue":
                        actionObject = new WaitUntilVariableReachesNumericValue(dParameters.VariableName, dParameters.Comparison, dParameters.ThresholdValue, new GetVariable(), Convert.ToInt32(dParameters.CheckInterval));
                        break;
                    case "CalculateBearingToCoordinates":
                        actionObject = new CalculateBearingToCoordinates(dParameters.FinalLatitude, dParameters.FinalLongitude, new GetVariable());
                        break;
                    case "CalculateDistanceToCoordinates":
                        actionObject = new CalculateDistanceToCoordinates(dParameters.FinalLatitude, dParameters.FinalLongitude, new GetVariable());
                        break;
                    case "FlightPositionLogger":
                        actionObject = new FlightPositionLogger(dParameters.LoggingTimeSeconds, dParameters.LoggingPeriodSeconds, new GetVariable(), dParameters.LogInNoLockingBackgroundMode);
                        break;
                    case "FlightPositionLoggerStop":
                        actionObject = new FlightPositionLoggerStop();
                        break;
                    default:
                        break;

                }





                /*
                actionObject = JsonConvert.DeserializeObject(parameters, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                actionObject.getVariable = new GetVariable();
                */
            }

            s_Name = name;
            s_UniqueID = uniqueID;
            e_Status = status;
            s_Parameters = JsonConvert.SerializeObject(actionObject, Formatting.Indented);
            s_Result = new ActionResult("", null);
            o_Object = actionObject;
            b_isValidated = false;
            b_isAuxiliary = isAuxiliary;
            b_stopOnError = stopOnError;
            o_AutomationFile = automationFile;
        }

        public FSAutomatorAction(string name, ActionStatus status)
        {
            s_Name = name;
            e_Status = status;
        }

        public FSAutomatorAction()
        {

        }

        [JsonProperty(Required = Required.Default)]
        public string Name
        {
            get { return s_Name; }

            set
            {
                s_Name = value;
                RaisePropertyChanged("Name");
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public int Id
        {
            get { return i_Id; }

            set
            {
                i_Id = value;
                RaisePropertyChanged("Id");
            }
        }

        [JsonProperty(Required = Required.Default)]
        public bool IsAuxiliary
        {
            get { return b_isAuxiliary; }

            set
            {
                b_isAuxiliary = value;
                RaisePropertyChanged("IsAuxiliary");
            }
        }

        [JsonProperty(Required = Required.Default)]
        public bool StopOnError
        {
            get { return b_stopOnError; }

            set
            {
                b_stopOnError = value;
                RaisePropertyChanged("StopOnError");
            }
        }

        [JsonProperty(Required = Required.Default)]
        public AutomationFile AutomationFile
        {
            get { return o_AutomationFile; }

            set
            {
                o_AutomationFile = value;
                RaisePropertyChanged("AutomationFile");
            }
        }

        [JsonProperty(Required = Required.Default)]
        public string UniqueID
        {
            get { return s_UniqueID; }

            set
            {
                s_UniqueID = value;
                RaisePropertyChanged("UniqueID");
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public ActionStatus Status
        {
            get { return e_Status; }

            set
            {
                e_Status = value;
                RaisePropertyChanged("Status");
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public bool IsCurrent
        {
            get { return b_isCurrent; }

            set
            {
                b_isCurrent = value;
                RaisePropertyChanged("IsCurrent");
            }
        }


        [JsonProperty(Required = Required.Default)]
        public string Parameters
        {
            get { return s_Parameters.Replace("\r", "").Replace("\n", ""); }

            set
            {
                s_Parameters = value;
                Type actionType = Utils.GetType(String.Format("FSAutomator.Backend.Actions.{0}", Name));
                try
                {       //mirar, perquè al deserialitzar no s'injecta interfaç
                    ActionObject = JsonConvert.DeserializeObject(value, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                }
                catch
                {
                    Trace.WriteLine("Malformed JSON");
                }
                RaisePropertyChanged("Parameters");
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public string ParametersBeautified
        {
            get { return s_Parameters; }
            set
            {
                Parameters = value;
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public ActionResult Result
        {
            get
            {
                return s_Result;
            }

            set
            {
                s_Result = value;
                RaisePropertyChanged("Result");
            }
        }

        [JsonProperty(Required = Required.Default)]
        public object ActionObject
        {
            get { return o_Object; }

            set
            {
                o_Object = value;
                RaisePropertyChanged("ActionObject");
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public string MainFilePath
        {
            get { return s_mainFilePath; }
            set
            {
                s_mainFilePath = value;
                RaisePropertyChanged("MainFilePath");
            }
        }

        [JsonProperty(Required = Required.Default)]
        [JsonIgnore]
        public bool IsValidated
        {
            get { return b_isValidated; }

            set
            {
                b_isValidated = value;
                RaisePropertyChanged("IsValidated");
            }
        }


        [JsonIgnore]
        public string ValidationOutcome
        {
            get { return s_validationOutcome; }

            set
            {
                s_validationOutcome = value;
                RaisePropertyChanged("ValidationOutcome");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                Task.Run(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)));
            }
        }

        public enum ActionStatus
        {
            Pending,
            Running,
            Done
        }



    }
}
