using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using FSAutomator.BackEnd.Configuration;

namespace FSAutomator.ViewModel
{
    public class AddActionViewModel : INotifyPropertyChanged
    {

        private AvailableActions l_AvailableActions;
        private string s_AvailableActionsName;
        private string s_SerializedJSON;


        private ICommand? b_ButtonOK;

        private List<Parameter> l_ActionParameters;
        private Parameter s_SActionParameters;

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
                writer.WritePropertyName("Parameters");
                writer.WriteStartObject();
                foreach (Parameter param in ActionParameters)
                {
                    writer.WritePropertyName(param.Name);
                    writer.WriteValue(param.Value);
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
