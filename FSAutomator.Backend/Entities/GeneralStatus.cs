using System.ComponentModel;

namespace FSAutomator.Backend.Entities
{
    public sealed class GeneralStatus : INotifyPropertyChanged
    {
        private bool b_IsConnectedToSim { get; set; } = false;
        private bool b_IsAutomationFullyValidated { get; set; }
        private List<string> l_ValidationIssues { get; set; }
        private bool b_GeneralErrorHasOcurred { get; set; } = false;

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

        public bool GeneralErrorHasOcurred
        {
            get { return this.b_GeneralErrorHasOcurred; }

            set { this.b_GeneralErrorHasOcurred = value; }
        }

        public void ReportError(InternalMessage msg)
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
