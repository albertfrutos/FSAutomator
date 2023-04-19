using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FlightSimulator.SimConnect;
using static FSAutomator.SimConnectInterface.Entities;



namespace FSAutomator.SimConnectInterface
{
    public class ShortcutActions
    {
        internal SimConnect Connection { get; set; } = null;

        private AutoResetEvent hold = new AutoResetEvent(false);
        private string returnString = null;

        public ShortcutActions()
        {
            
        }

        public string GetLoadedAircraftCfgFilePath()
        {
            returnString = null;
            GetLoadedAircraftCfgFilePath(RecoverSimConnectRecvSystemStateSzString);
            hold.WaitOne();

            return returnString;
        }

        private void RecoverSimConnectRecvSystemStateSzString(SimConnect sc, SIMCONNECT_RECV_SYSTEM_STATE data)
        {
            returnString = data.szString;
            hold.Set();
        }

        private void GetLoadedAircraftCfgFilePath(Action<SimConnect, SIMCONNECT_RECV_SYSTEM_STATE> method)
        {
            Connection.OnRecvSystemState += new SimConnect.RecvSystemStateEventHandler(method);
            Connection.RequestSystemState(DATA_REQUESTS.REQUEST_1, "AircraftLoaded");
        }
    }
}
