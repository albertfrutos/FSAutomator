using FSAutomator.BackEnd.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd
{
    public class GeneralStatus
    {
        public bool isConnectedToSim { get; set; }
        public bool isAutomationFullyValidated { get; set; } //note fer i refactor
        public List<string> validationIssues { get; set; }


        public GeneralStatus()
        {
        }



    }
}
