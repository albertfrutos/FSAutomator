﻿using FSAutomator.Backend.Utilities;
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
        private string s_Result;
        private object o_Object;
        private bool b_isValidated;
        private string s_validationOutcome;

        public FSAutomatorAction(string name, string uniqueID, string status, string parameters, object actionObject)
        {
            s_Name = name;
            s_UniqueID = uniqueID;
            s_Status = status;
            s_Parameters = JsonConvert.SerializeObject(actionObject);
            s_Result = "";
            o_Object = actionObject;
            b_isValidated = false;
        }

        public FSAutomatorAction(string name, string status)
        {
            s_Name = name;
            s_Status= status;
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
                    ActionObject = JsonConvert.DeserializeObject(value, actionType);
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

        public string Result
        {
            get { return s_Result.Replace("\r", "").Replace("\n", ""); }

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
