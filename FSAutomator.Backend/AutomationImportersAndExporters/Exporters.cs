using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Configuration;
using System.Collections.ObjectModel;
using System.IO.Compression;

namespace FSAutomator.BackEnd.AutomationImportersAndExporters
{
    internal class Exporters
    {
        public ApplicationConfig Config = ApplicationConfig.GetInstance;

        internal InternalMessage ExportAutomation(string fileName, ObservableCollection<FSAutomatorAction> actionList, AutomationFile automationFile)
        {
            var exportPath = Config.ExportFolder;

            if (actionList is not null && actionList.Any())
            {
                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }

                if (actionList[0].Name == "DLLAutomation")
                {
                    var DLLFileName = (actionList[0].ActionObject as FSAutomator.Backend.Automators.ExternalAutomator).DLLName;
                    var SavedDLLFileName = Path.Combine(exportPath, DLLFileName);
                    ExportDLLAutomation(SavedDLLFileName, automationFile);
                }
                else if (automationFile.IsPackage)   //this is a JSON, maybe within a pack, maybe standalone
                {


                    List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);

                    if (dllFilesInAction.Any())
                    {
                        var packageName = automationFile.PackageName;
                        var automationFileName = automationFile.FileName;

                        bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction, Path.Combine(Config.AutomationsFolder, packageName));

                        if (allDLLsExist)
                        {
                            if (!(fileName.Length > 0))
                            {
                                return new InternalMessage("Please enter an automation name", "Error", true);
                            }

                            var json = Utils.GetJSONTextFromAutomationList(actionList, packageName);
                            ExportPack(fileName, packageName, automationFile, json);

                        }
                    }
                }
                else //standalone json
                {
                    if (!(fileName.Length > 0))
                    {
                        return new InternalMessage("Please enter an automation name", "Error", true);
                    }

                    var jsonFileName = Path.Combine(exportPath, Path.GetFileNameWithoutExtension(fileName) + ".json");
                    var json = Utils.GetJSONTextFromAutomationList(actionList);
                    List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);
                    var existingDLLFilesInAction = dllFilesInAction.Where(x => System.IO.File.Exists(x)).ToList();

                    if (existingDLLFilesInAction.Count == dllFilesInAction.Count)
                    {
                        ExportJson(jsonFileName, json, dllFilesInAction, exportPath);
                    }
                    else
                    {
                        return new InternalMessage("The json file has missing DLL files", "Error", true);
                    }
                }

                return new InternalMessage("Export finished", "", false);

            }

            return new InternalMessage("There is nothing to save", "Error", true);

        }

        private void ExportPack(string filename, string packageName, AutomationFile automationFile, string json)
        {
            var tempDirPath = Path.Combine(Config.TempFolder, filename);
            var JSONPath = Path.Combine(tempDirPath, automationFile.FileName);
            var newJSONPath = Path.Combine(tempDirPath, filename + ".json");

            Directory.CreateDirectory(tempDirPath);

            Utils.CopyFullDirectory(Path.Combine(Config.AutomationsFolder, packageName).ToString(), tempDirPath);
            Utils.DeleteFilesFromDirectoryWithExtension(tempDirPath, "bak");

            File.Delete(JSONPath);

            File.WriteAllText(newJSONPath, json);

            var exportsPath = Path.Combine(Directory.GetParent(filename).ToString(), Config.ExportFolder);
            var zipPath = Path.Combine(exportsPath, filename + ".zip");

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(tempDirPath, zipPath, CompressionLevel.NoCompression, false);

            Directory.Delete(tempDirPath, true);
        }

        private void ExportDLLAutomation(string saveDLL, AutomationFile automationFile)
        {
            File.Copy(automationFile.FilePath, saveDLL);
        }

        private void ExportJson(string filename, string json, List<string> dllFilesInAction, string exportPath)
        {
            File.WriteAllText(filename, json);

            foreach (string dllFile in dllFilesInAction)
            {
                File.Copy(dllFile, Path.Combine(exportPath, Path.GetFileName(dllFile)));
            }

        }



    }
}
