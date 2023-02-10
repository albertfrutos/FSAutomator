using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace FSAutomator.Backend.Actions
{
    public class ExecuteCodeFromDLL : IAction
    {
        public string DLLName { get; set; }
       
        [JsonIgnore]
        public string DLLPath { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public bool IncludeAsExternalAutomator { get; set; } = false;

        public string DLLPackageFolder = "";

        AutoResetEvent evento = new AutoResetEvent(false);

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

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            var memoryRegisters = (Dictionary<string, string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);
            var lastValue = sender.GetType().GetField("lastOperationValue").GetValue(sender).ToString();
            var actionsList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.DLLPackageFolder, this.DLLName);
            var DLL = Assembly.LoadFrom(path);
            string classPath = String.Format("FSAutomator.ExternalAutomation.{0}", this.ClassName);
            var type = DLL.GetType(classPath);
            object instance = Activator.CreateInstance(type);
            var result = instance.GetType().GetMethod(this.MethodName).Invoke(instance, new object[] { this, connection, evento, memoryRegisters, lastValue, actionsList });
            evento.WaitOne();
            return new ActionResult(result.ToString(), result.ToString());
        }
    }
}
