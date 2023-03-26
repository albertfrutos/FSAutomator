using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.Backend.Actions
{
    public class ActionBase
    {
        internal IGetVariable getVariable;

        public ActionBase()
        {
        }

        public ActionBase(IGetVariable getVariable, string variableName)
        {
            this.getVariable = getVariable;
            this.getVariable.VariableName = variableName;

        }
    }
}
