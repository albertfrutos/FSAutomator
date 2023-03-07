using FSAutomator.Backend.Entities.FlightModelEntities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using Microsoft.FlightSimulator.SimConnect;
using static FSAutomator.Backend.Entities.CommonEntities;

namespace FSAutomator.Backend.Entities
{
    public class FlightModel
    {
        public ReferenceSpeeds ReferenceSpeeds { get; set; }

        private const string aircraftCfgFileName = "aircraft.cfg";
        private const string flightModelCfgFileName = "flight_model.cfg";
        private string flightModelPath;

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
            var baseFSPathOfficial = ApplicationConfig.GetInstance.FSPackagesPaths.FSPathOfficial;
            var baseFSPathCommunity = ApplicationConfig.GetInstance.FSPackagesPaths.FSPathCommunity;

            List<string> installedAircrafts = SearchFileInAllDirectories(baseFSPathOfficial, aircraftCfgFileName);
            installedAircrafts.AddRange(SearchFileInAllDirectories(baseFSPathCommunity, aircraftCfgFileName));

            var currentAircraftCfgPath = installedAircrafts.Where(z => z.EndsWith(data.szString, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToString();

            if (currentAircraftCfgPath != "")
            {
                this.flightModelPath = Path.Combine(Path.GetDirectoryName(currentAircraftCfgPath), flightModelCfgFileName);

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
