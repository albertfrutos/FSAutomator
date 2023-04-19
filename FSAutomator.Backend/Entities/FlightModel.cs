using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.SimConnectInterface;
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

        public FlightModel(ISimConnectBridge Connection)
        {
            if (Connection != null)
            {
                var path = Connection.ShortcutActions.GetLoadedAircraftCfgFilePath();
                LoadFlightModelData(path);
            }
        }

        private void LoadFlightModelData(string path)
        {
            var baseFSPathOfficial = ApplicationConfig.GetInstance.FSPackagesPaths.FSPathOfficial;
            var baseFSPathCommunity = ApplicationConfig.GetInstance.FSPackagesPaths.FSPathCommunity;

            List<string> installedAircrafts = SearchFileInAllDirectories(baseFSPathOfficial, aircraftCfgFileName);
            installedAircrafts.AddRange(SearchFileInAllDirectories(baseFSPathCommunity, aircraftCfgFileName));

            var currentAircraftCfgPath = installedAircrafts.Where(z => z.EndsWith(path, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().ToString();

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
