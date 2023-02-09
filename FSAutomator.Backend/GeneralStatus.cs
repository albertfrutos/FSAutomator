using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd
{
    internal class GeneralStatus
    {
        internal bool isConnectedToSim { get; set; }
        internal bool isAutomationFullyValidated { get; set; } //note fer i refactor
        internal List<string> validationIssues { get; set; }

        internal GeneralStatus()
        {
            isConnectedToSim = false;
            isAutomationFullyValidated = false;
            validationIssues = new List<string>();
        }
    }
}
