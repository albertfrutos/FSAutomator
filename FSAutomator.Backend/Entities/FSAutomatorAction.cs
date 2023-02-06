using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Entities;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;

namespace FSAutomator.Backend.Entities
{
    public class FSAutomatorAction : INotifyPropertyChanged
    {
        private string s_Name;
        private string s_UniqueID;
        private string s_Status;
        private string s_Parameters;
        private ActionResult s_Result;
        private object o_Object;
        private bool b_isValidated;
        private bool b_isAuxiliary;
        private bool b_stopOnError;
        private string s_validationOutcome;

        public FSAutomatorAction(string name, string uniqueID, string status, string parameters, object actionObject, bool isAuxiliary, bool stopOnError)
        {
            s_Name = name;
            s_UniqueID = uniqueID;
            s_Status = status;
            s_Parameters = JsonConvert.SerializeObject(actionObject);
            s_Result = new ActionResult("", null);
            o_Object = actionObject;
            b_isValidated = false;
            b_isAuxiliary = isAuxiliary;
            b_stopOnError = stopOnError;
        }

        public FSAutomatorAction(string name, string status)
        {
            s_Name = name;
            s_Status= status;
        }
        
        public FSAutomatorAction()
        {
            
        }

        public string Name
        {
            get { return s_Name; }

            set
            {
                s_Name = value;
                RaisePropertyChanged("Name");
            }
        }
        
        public bool IsAuxiliary
        {
            get { return b_isAuxiliary; }

            set
            {
                b_isAuxiliary = value;
                RaisePropertyChanged("IsAuxiliary");
            }
        }
        
        public bool StopOnError
        {
            get { return b_stopOnError; }

            set
            {
                b_stopOnError = value;
                RaisePropertyChanged("StopOnError");
            }
        }

        public string UniqueID
        {
            get { return s_UniqueID; }

            set
            {
                s_UniqueID = value;
                RaisePropertyChanged("UniqueID");
            }
        }

        public string Status
        {
            get { return s_Status; }

            set
            {
                s_Status = value;
                RaisePropertyChanged("Status");
            }
        }

        public string Parameters
        {
            get { return s_Parameters.Replace("\r", "").Replace("\n", ""); }

            set
            {
                s_Parameters = value;
                Type actionType = Utils.GetType(String.Format("FSAutomator.Backend.Actions.{0}", Name));
                try
                {
                    ActionObject = JsonConvert.DeserializeObject(value, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                }
                catch
                {
                    Trace.WriteLine("Malformed JSON");
                }
                RaisePropertyChanged("Parameters");
            }
        }

        public string ParametersBeautified
        {
            get { return s_Parameters; }
            set
            {
                Parameters = value;
            }
        }

        public ActionResult Result
        {
            get
            {
                return s_Result; // s_Result.VisibleResult.Replace("\r", "").Replace("\n", "");
            }

            set
            {
                s_Result = value;
                RaisePropertyChanged("Result");
            }
        }

        public object ActionObject
        {
            get { return o_Object; }

            set
            {
                o_Object = value;
                RaisePropertyChanged("ActionObject");
            }
        }
        
        public bool IsValidated
        {
            get { return b_isValidated; }

            set
            {
                b_isValidated = value;
                RaisePropertyChanged("IsValidated");
            }
        }

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


    }
}
