﻿using FSAutomator.Backend.AutomatorInterface;
using FSAutomator.Backend.Entities;
using FSAutomator.SimConnectInterface;
using System.Reflection;

namespace FSAutomator.Backend.Automators
{
    public class ExternalAutomator
    {
        public string DLLName { get; set; }
        public string DLLPath { get; set; }

        internal AutoResetEvent finishEvent = new AutoResetEvent(false);

        internal FSAutomatorInterface externalAutomatorInterface = null;

        public ExternalAutomator()
        {

        }
        public ExternalAutomator(string DLLName, string DLLPath)
        {
            this.DLLName = DLLName;
            this.DLLPath = DLLPath;
        }

        public ActionResult ExecuteAction(Automator sender, ISimConnectBridge connection)
        {
            this.externalAutomatorInterface = new FSAutomatorInterface(sender, connection, finishEvent);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.DLLPath);
            var DLL = Assembly.LoadFile(path);

            var type = DLL.GetType("FSAutomator.ExternalAutomation.ExternalAutomation");
            object instance = Activator.CreateInstance(type);

            var result = instance.GetType().GetMethod("Execute").Invoke(instance, new object[] { this.externalAutomatorInterface });

            ExternalAutomationFinishActions();

            return (ActionResult)result;
        }

        private void ExternalAutomationFinishActions()
        {
            this.finishEvent.Set();
            this.externalAutomatorInterface.AdvancedActionsManager.FlightPositionLoggerStop(false);
        }
    }
}
