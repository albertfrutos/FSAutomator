using FSAutomator.BackEnd.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd.Entities
{
    public sealed class GeneralStatus : INotifyPropertyChanged
    {
        private bool b_IsConnectedToSim { get; set; } = false;
        private bool b_IsAutomationFullyValidated { get; set; } //note fer i refactor
        private List<string> l_ValidationIssues { get; set; }

        private static GeneralStatus instance = null;

        public event EventHandler<InternalMessage> ReportErrorEvent;

        public event EventHandler<bool> ConnectionStatusChangeEvent;

        public static GeneralStatus GetInstance
        {
            get
            { 
                if (instance == null)
                {
                    instance = new GeneralStatus();
                }
                return instance;
            }
        }

        public bool IsConnectedToSim
        {
            get { return this.b_IsConnectedToSim; }
            set
            {
                if (IsConnectedToSim != value)
                {
                    this.b_IsConnectedToSim = value;
                    ConnectionChangeEvent();
                }
                RaisePropertyChanged("IsConnectedToSim");
            }
        }

        public bool IsAutomationFullyValidated
        {
            get { return this.b_IsAutomationFullyValidated; }
            set
            {
                this.b_IsAutomationFullyValidated = value;
                RaisePropertyChanged("IsAutomationFullyValidated");
            }
        }

        public List<string> ValidationIssues
        {
            get { return this.l_ValidationIssues; }
            set
            {
                this.l_ValidationIssues = value;
                RaisePropertyChanged("ValidationIssues");
            }
        }

        public void ReportError(InternalMessage msg)  // note add to public interface
        {
            if (this.ReportErrorEvent != null)
            {
                this.ReportErrorEvent.Invoke(this, msg);
            }
        }
        
        private void ConnectionChangeEvent() 
        {
            if (this.ConnectionStatusChangeEvent != null)
            {
                this.ConnectionStatusChangeEvent.Invoke(this, this.IsConnectedToSim);
                Trace.WriteLine("csc inv");
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
