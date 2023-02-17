using FSAutomator.BackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd
{
    public sealed class GeneralStatus
    {
        private bool b_IsConnectedToSim { get; set; } = false;
        private bool b_IsAutomationFullyValidated { get; set; } //note fer i refactor
        private List<string> l_ValidationIssues { get; set; }

        private static GeneralStatus instance = null;

        public event EventHandler<string> Report;

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
            set { this.b_IsConnectedToSim = value;}
        }

        public bool IsAutomationFullyValidated
        {
            get { return this.b_IsAutomationFullyValidated; }
            set { this.b_IsAutomationFullyValidated = value;}
        }

        public List<string> ValidationIssues
        {
            get { return this.l_ValidationIssues; }
            set { this.l_ValidationIssues = value; }
        }

        public void ReportEvent(string data)  // note add to public interface
        {
            if (this.Report != null)
            {
                this.Report.Invoke(this, data);
            }
        }

    }
}
