using FSAutomator.Backend.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FSAutomator.ViewModel
{
    public class AddActionViewModel : INotifyPropertyChanged
    {

        private AvailableActions l_AvailableActions;
        private Parameter s_SActionParameters;
        private List<AvailableFixedBoolItem> l_FixedBoolItems;
        private List<Parameter> l_ActionParameters;
        private string s_UniqueID = null;
        private string s_FixedBoolItemName;
        private string s_AvailableActionsName;
        private string s_SerializedJSON;


        private ICommand? b_ButtonOK;



        public ICommand ButtonOK
        {
            get
            {
                return b_ButtonOK;
            }
            set
            {
                b_ButtonOK = value;
            }
        }

        public AddActionViewModel()
        {
            AvailableActions = new AvailableActions().GetAvailableActions();
            FixedBoolItems = new AvailableFixedBoolItems().GetAvailableItems();

            ButtonOK = new RelayCommand(new Action<object>(BuildNewAction));

        }

        private void BuildNewAction(object obj)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("Name");
                writer.WriteValue(SAvailableActionName);

                writer.WritePropertyName("UniqueID");
                writer.WriteValue(SUniqueID);

                writer.WritePropertyName("IsAuxiliary");
                var isAuxiliary = l_FixedBoolItems.Where(x => x.Name == "IsAuxiliary").Select(y => y.Value.ToString()).First();
                writer.WriteValue(isAuxiliary);

                writer.WritePropertyName("StopOnError");
                var stopOnError = l_FixedBoolItems.Where(x => x.Name == "StopOnError").Select(y => y.Value.ToString()).First();
                writer.WriteValue(stopOnError);

                writer.WritePropertyName("ParallelLaunch");
                var parallelLaunch = l_FixedBoolItems.Where(x => x.Name == "ParallelLaunch").Select(y => y.Value.ToString()).First();
                writer.WriteValue(parallelLaunch);

                writer.WritePropertyName("Parameters");
                writer.WriteStartObject();
                if (ActionParameters != null)
                {
                    foreach (Parameter param in ActionParameters)
                    {
                        writer.WritePropertyName(param.Name);
                        writer.WriteValue(param.Value);
                    }
                }
                writer.WriteEndObject();
                writer.WriteEnd();
            }

            SerializedJSON = sb.ToString();
        }

        public AvailableActions AvailableActions
        {
            get
            {
                return l_AvailableActions;

            }
            set
            {
                l_AvailableActions = value;
                RaisePropertyChanged("AvailableActions");
            }

        }

        public List<AvailableFixedBoolItem> FixedBoolItems
        {
            get
            {
                return l_FixedBoolItems;

            }
            set
            {
                l_FixedBoolItems = value;
                RaisePropertyChanged("FixedBoolItems");
            }

        }

        public string SerializedJSON
        {
            get
            {
                return s_SerializedJSON;

            }
            set
            {
                s_SerializedJSON = value;
                RaisePropertyChanged("SerializedJSON");
            }

        }


        public List<Parameter> ActionParameters
        {
            get
            {
                return l_ActionParameters;

            }
            set
            {
                l_ActionParameters = value;
                RaisePropertyChanged("ActionParameters");
            }

        }

        public Parameter SActionParameters
        {
            get
            {
                return s_SActionParameters;

            }
            set
            {
                s_SActionParameters = value;
                BuildNewAction(null);
                RaisePropertyChanged("SActionParameters");
            }

        }

        public string[] AvailableActionsNames
        {
            get
            {
                return l_AvailableActions.FSAutomatorAvailableActions.Select(x => x.Name).ToArray();

            }

        }
        public string SAvailableActionName
        {
            get
            {
                return s_AvailableActionsName;

            }
            set
            {
                s_AvailableActionsName = value;
                ActionParameters = AvailableActions.FSAutomatorAvailableActions.Where(x => x.Name == value).First().Parameters;
                BuildNewAction(null);
                RaisePropertyChanged("SAvailableActionName");
            }

        }

        public string SFixedBoolItemName
        {
            get
            {
                return s_FixedBoolItemName;

            }
            set
            {
                s_FixedBoolItemName = value;
                BuildNewAction(null);
                RaisePropertyChanged("SFixedBoolItemName");
            }

        }

        private string SUniqueID
        {
            get
            {
                s_UniqueID = s_UniqueID ?? Guid.NewGuid().ToString();
                return s_UniqueID;
            }
        }




        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                Task.Run(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)));
            }
        }





    }


}
