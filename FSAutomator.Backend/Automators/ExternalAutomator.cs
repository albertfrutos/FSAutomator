using Microsoft.FlightSimulator.SimConnect;
using System.Reflection;

namespace FSAutomator.Backend.Automators
{
    public class ExternalAutomator
    {
        public string DLLName { get; set; }
        public string DLLPath { get; set; }

        public ExternalAutomator()
        {
            
        }
        public ExternalAutomator(string DLLName, string DLLPath)
        {
            this.DLLName = DLLName;
            this.DLLPath = DLLPath;
        }

        public void ExecuteAction(object sender, SimConnect connection, EventHandler<string> ReturnValueEvent, EventHandler UnlockNextStep)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.DLLPath);
            var DLL = Assembly.LoadFile(path);

            var type = DLL.GetType("FSAutomator.ExternalAutomation.ExternalAutomation");
            object instance = Activator.CreateInstance(type);

            var a = instance.GetType().GetMethod("Execute").Invoke(instance, new object[] { this, connection, ReturnValueEvent, UnlockNextStep });
        }
    }
}
