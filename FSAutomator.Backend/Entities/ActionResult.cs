using System.ComponentModel;

namespace FSAutomator.Backend.Entities
{
    public class ActionResult : INotifyPropertyChanged
    {
        private string s_VisibleResult = "";
        private string s_ComputedResult = "";
        private bool b_Error = false;
        private dynamic o_ReturnObject = null;

        public ActionResult()
        {

        }
        public ActionResult(string visibleResult, string computedResult, bool error = false, dynamic returnObject = null)
        {
            this.VisibleResult = visibleResult;
            this.ComputedResult = computedResult;
            this.Error = error;
            this.ReturnObject = returnObject;
        }

        public string VisibleResult
        {
            get { return s_VisibleResult; }

            set
            {
                s_VisibleResult = value;
                RaisePropertyChanged("VisibleResult");
            }
        }

        public string ComputedResult
        {
            get { return s_ComputedResult; }

            set
            {
                s_ComputedResult = value;
                RaisePropertyChanged("ComputedResult");
            }
        }

        public bool Error
        {
            get { return b_Error; }

            set
            {
                b_Error = value;
                RaisePropertyChanged("Error");
            }
        }

        public dynamic ReturnObject
        {
            get { return o_ReturnObject; }

            set
            {
                o_ReturnObject = value;
                RaisePropertyChanged("ReturnObject");
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
