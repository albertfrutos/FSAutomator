using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics;
using System.Reflection;

namespace FSAutomator.Backend.Automators
{
    public class ExternalAutomator
    {
        public string DLLName { get; set; }
        public string DLLPath { get; set; }

        internal AutoResetEvent finishEvent = new AutoResetEvent(false);

        public ExternalAutomator()
        {

        }
        public ExternalAutomator(string DLLName, string DLLPath)
        {
            this.DLLName = DLLName;
            this.DLLPath = DLLPath;
        }

        public ActionResult ExecuteAction(Automator sender, SimConnect connection)
        {
            var externalAutomatorInterface = new FSAutomatorInterface(sender, connection, finishEvent);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.DLLPath);
            var DLL = Assembly.LoadFile(path);

            var type = DLL.GetType("FSAutomator.ExternalAutomation.ExternalAutomation");
            object instance = Activator.CreateInstance(type);

            string result = instance.GetType().GetMethod("Execute").Invoke(instance, new object[] { externalAutomatorInterface }).ToString();

            finishEvent.Set();

            Trace.WriteLine("fired!");

            return new ActionResult(result.ToString(), result.ToString());
        }
    }
}
