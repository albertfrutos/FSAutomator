using FSAutomator.Backend.Entities;
using FSAutomator.BackEnd.Entities;
using Microsoft.FlightSimulator.SimConnect;
using System.Reflection;

namespace FSAutomator.Backend.Automators
{
    public class ExternalAutomator
    {
        public string DLLName { get; set; }
        public string DLLPath { get; set; }

        internal AutoResetEvent evento = new AutoResetEvent(false);

        public ExternalAutomator()
        {
            
        }
        public ExternalAutomator(string DLLName, string DLLPath)
        {
            this.DLLName = DLLName;
            this.DLLPath = DLLPath;
        }

        public ActionResult ExecuteAction(object sender, SimConnect connection)
        {
            //note mirar com treure el Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location .... File.Info....FullName
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.DLLPath);
            var DLL = Assembly.LoadFile(path);

            var type = DLL.GetType("FSAutomator.ExternalAutomation.ExternalAutomation");
            object instance = Activator.CreateInstance(type);

            string result = instance.GetType().GetMethod("Execute").Invoke(instance, new object[] { this, connection, evento }).ToString();

            return new ActionResult(result.ToString(), result.ToString());
        }
    }
}
