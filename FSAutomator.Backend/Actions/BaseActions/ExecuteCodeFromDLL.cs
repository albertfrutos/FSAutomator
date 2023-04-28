using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Configuration;
using FSAutomator.SimConnectInterface;
using Newtonsoft.Json;
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

        public string PackFolder = "";

        [JsonIgnore]
        public ApplicationConfig Config = ApplicationConfig.GetInstance;

        AutoResetEvent finishEvent = new AutoResetEvent(false);

        public ExecuteCodeFromDLL()
        {

        }
        internal ExecuteCodeFromDLL(string DLLName, string DLLPath, string ClassName, string MethodName, bool IncludeAsExternalAutomator)
        {
            this.DLLName = DLLName;
            this.DLLPath = DLLPath;
            this.PackFolder = "";
            this.ClassName = ClassName;
            this.MethodName = MethodName;
            this.IncludeAsExternalAutomator = IncludeAsExternalAutomator;
        }

        public ActionResult ExecuteAction(object sender, ISimConnectBridge connection)
        {
            var memoryRegisters = (sender as Automator).MemoryRegisters;
            var lastValue = (sender as Automator).lastOperationValue;
            var actionsList = (sender as Automator).ActionList;

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Config.AutomationsFolder, this.PackFolder, this.DLLName);
            var DLL = Assembly.LoadFrom(path);
            string classPath = String.Format("FSAutomator.ExternalAutomation.{0}", this.ClassName);
            var type = DLL.GetType(classPath);
            object instance = Activator.CreateInstance(type);
            var result = instance.GetType().GetMethod(this.MethodName).Invoke(instance, new object[] { this, connection, finishEvent, memoryRegisters, lastValue, actionsList });
            finishEvent.WaitOne();
            return new ActionResult(result.ToString(), result.ToString());
        }
    }
}
