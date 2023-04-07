using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using System.Diagnostics;
using System.IO.Compression;

namespace FSAutomator.BackEnd.AutomationImportersAndExporters
{
    internal class Importers
    {
        public ApplicationConfig Config = ApplicationConfig.GetInstance;

        internal void ImportAutomationFromFilePath(string filepath)
        {
            var filetype = Path.GetExtension(filepath);

            switch (filetype)
            {
                case ".json":
                    ImportJSONAutomation(filepath);
                    break;
                case ".dll":
                    ImportDLLAutomation(filepath);
                    break;
                case ".zip":
                    ImportZipAutomation(filepath);
                    break;
            }
        }

        private void ImportZipAutomation(string ZIPPath)
        {
            var tempDirName = Path.GetFileNameWithoutExtension(ZIPPath);
            var tempDirPath = Path.Combine(Config.TempFolder, tempDirName);

            ZipFile.ExtractToDirectory(ZIPPath, tempDirPath);

            var jsonFileName = Directory.GetFiles(tempDirPath, "*.json");

            if (jsonFileName.Count() > 1)
            {
                Trace.WriteLine("Only packages with one JSON file are supported");
            }
            else
            {
                var JSONPath = jsonFileName[0];

                var automationFile = new AutomationFile(Path.GetFileName(JSONPath), Path.GetFileNameWithoutExtension(JSONPath), "", JSONPath, "", true);

                var actionList = Utils.GetActionsList(automationFile, false);

                if (actionList is null)
                {
                    GeneralStatus.GetInstance.ReportStatus(new InternalMessage("The automation file does not have contain actions", true));
                    return;
                }

                List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);

                bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction);

                if (allDLLsExist)
                {
                    var automationsTargetDir = Path.Combine(Config.AutomationsFolder, tempDirName);
                    Utils.CopyFullDirectory(tempDirPath, automationsTargetDir);
                }
            }

            Directory.Delete(tempDirPath, true);
        }

        private void ImportDLLAutomation(string filepath)
        {

            var dllFileName = Path.GetFileName(filepath);

            var destinationDLLPath = Path.Combine(Config.AutomationsFolder, dllFileName);

            if (!File.Exists(destinationDLLPath))
            {
                File.Copy(filepath, destinationDLLPath);
            }
        }

        private void ImportJSONAutomation(string JSONPath)
        {
            var JSONFileName = Path.GetFileName(JSONPath);

            var automationFile = new AutomationFile(JSONFileName, "", "", JSONPath, "");

            var actionList = Utils.GetActionsList(automationFile,false);

            if (actionList is null)
            {
                var exMessage = String.Format("There was a problem while processing the action list for {0}", JSONFileName);

                return;

            }

            List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);

            //JSON has references to DLL files, so it's considered like a pack
            if (dllFilesInAction.Any())
            {

                bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction);

                if (allDLLsExist)
                {
                    File.Copy(JSONPath, Path.Combine(Config.AutomationsFolder, JSONFileName));

                    foreach (string dllFilePath in dllFilesInAction)
                    {
                        var dllFileName = Path.GetFileName(dllFilePath);

                        var destinationDLLPath = Path.Combine(Config.AutomationsFolder, dllFileName);

                        if (!File.Exists(destinationDLLPath))
                        {
                            File.Copy(dllFilePath, destinationDLLPath);
                        }

                    }
                }
            }
            else  //Standalone JSON
            {
                var fileName = Path.GetFileName(JSONPath);
                File.Copy(JSONPath, Path.Combine(Config.AutomationsFolder, fileName));

            }

        }

    }
}
