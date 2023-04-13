using System.ComponentModel;

namespace FSAutomator.Backend.Entities
{
    public sealed class GeneralStatus : INotifyPropertyChanged
    {
        public event EventHandler<InternalMessage> ReportStatusEvent;
        public event EventHandler<bool> ConnectionStatusChangeEvent;

        private static GeneralStatus instance = null;

        private bool b_IsConnectedToSim { get; set; } = false;
        private bool b_IsAutomationFullyValidated { get; set; } = false;
        private List<string> l_ValidationIssues { get; set; }
        private bool b_GeneralErrorHasOcurred { get; set; } = false;


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
                this.IsAutomationFullyValidated = value.Any();

                RaisePropertyChanged("ValidationIssues");
            }
        }

        public bool GeneralErrorHasOcurred
        {
            get { return this.b_GeneralErrorHasOcurred; }

            set { this.b_GeneralErrorHasOcurred = value; }
        }

        public void ReportStatus(InternalMessage internalMessage)
        {

            if (this.ReportStatusEvent != null)
            {
                this.ReportStatusEvent.Invoke(this, internalMessage);
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
