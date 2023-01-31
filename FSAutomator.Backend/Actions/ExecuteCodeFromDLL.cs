using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace FSAutomator.Backend.Actions
{
    public class ExecuteCodeFromDLL
    {
        public string DLLName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public bool IncludeAsExternalAutomator { get; set; } = false;

        public string DLLPackageFolder = "";

        public ExecuteCodeFromDLL()
        {
            
        }
        public ExecuteCodeFromDLL(string DLLName, string ClassName, string MethodName, bool IncludeAsExternalAutomator)
        {
            this.DLLName = DLLName;
            this.DLLPackageFolder = "";
            this.ClassName = ClassName;
            this.MethodName = MethodName;
            this.IncludeAsExternalAutomator = IncludeAsExternalAutomator;
        }

        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            var memoryRegisters = (Dictionary<string, string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);
            var lastValue = sender.GetType().GetField("lastOperationValue").GetValue(sender).ToString();
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            //note peta dll path al venir desde json de pack
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Automations", this.DLLPackageFolder, this.DLLName);
            var DLL = Assembly.LoadFrom(path);
            string classPath = String.Format("FSAutomator.ExternalAutomation.{0}", this.ClassName);
            var type = DLL.GetType(classPath);
            object instance = Activator.CreateInstance(type);
            instance.GetType().GetMethod(this.MethodName).Invoke(instance, new object[] { this, connection, ReturnValueEvent, UnlockNextStep, memoryRegisters, lastValue, actionsList });
        }
    }
}
