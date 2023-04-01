using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.Backend.Actions.Base
{
    public class ActionBase
    {
        internal IGetVariable getVariable;
        internal ISendEvent sendEvent;

        public ActionBase()
        {
        }

        public ActionBase(IGetVariable getVariable, ISendEvent sendEvent)
        {
            this.getVariable = getVariable;
            this.sendEvent = sendEvent;
        }
        
        public ActionBase(IGetVariable getVariable)
        {
            this.getVariable = getVariable;
        }
    }
}
