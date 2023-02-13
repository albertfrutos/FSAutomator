using FSAutomator.Backend;
using FSAutomator.Backend.Actions;
using FSAutomator.BackEnd.Entities;

namespace FSAutomator.Interface
{
    public class FSAutomatorInterface
    {
        private BackendMain backend = new BackendMain();
        public ActionResult GetVariable(string variableName)
        {
            var action = new GetVariable(variableName);
            action.ExecuteAction(backend.automator, backend.Connection);
        }
        
        /*
        
        ConditionalAction
        ExecuteCodeFromDLL
        ExpectVariableValue
        GetVariable
        MemoryRegisterRead
        MemoryRegisterWrite
        OperateValue
        SendEvent
        WaitSeconds
        WaitUntilVariableReachesNumericValue

        */

    }
}