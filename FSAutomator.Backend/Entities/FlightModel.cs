using FSAutomator.Backend.Entities.FlightModelEntities;
using FSAutomator.Backend.Utilities;
using Microsoft.FlightSimulator.SimConnect;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.Backend.Entities
{
    public class FlightModel
    {
        private string flightModelPath;
        public ReferenceSpeeds ReferenceSpeeds { get; set; }

        public FlightModel(SimConnect Connection)
        {
            if (Connection != null)
            {
                Connection.OnRecvSystemState += new SimConnect.RecvSystemStateEventHandler(GetAirCraftCfgPath);
                Connection.RequestSystemState(DATA_REQUESTS.REQUEST_1, "AircraftLoaded");
            }
        }

        private void GetAirCraftCfgPath(SimConnect Connection, SIMCONNECT_RECV_SYSTEM_STATE data)
        {
            var baseFSPathOfficial = @"C:\Users\Albert\AppData\Roaming\Microsoft Flight Simulator\Packages\Official";
            var baseFSPathCommunity = @"C:\Users\Albert\AppData\Roaming\Microsoft Flight Simulator\Packages\Community";

            List<string> installedAircrafts = SearchFileInAllDirectories(baseFSPathOfficial, "aircraft.cfg");
            installedAircrafts.AddRange(SearchFileInAllDirectories(baseFSPathCommunity, "aircraft.cfg"));

            var currentAircraftCfgPath = installedAircrafts.Where(z => z.EndsWith(data.szString, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToString();

            if (currentAircraftCfgPath != "")
            {
                this.flightModelPath = Path.Combine(Path.GetDirectoryName(currentAircraftCfgPath), "flight_model.cfg");

                IniFile ini = new IniFile(flightModelPath);
                this.ReferenceSpeeds = new ReferenceSpeeds(ini);
            }
        }

        private List<string> SearchFileInAllDirectories(string parentDirectory, string filename)
        {
            return Directory.GetFiles(parentDirectory, filename, SearchOption.AllDirectories).ToList();
        }

    }
}
