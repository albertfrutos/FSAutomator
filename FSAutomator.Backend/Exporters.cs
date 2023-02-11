using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSAutomator.BackEnd
{
    internal class Exporters
    {
        internal bool ExportAutomation(string fileName, string destinationPath, ObservableCollection<FSAutomatorAction> actionList, AutomationFile automationFile)
        {
            var exportPath = @"Exports";
            if (actionList is not null && actionList.Count() > 0)
            {
                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }

                if (actionList[0].Name == "DLLAutomation")
                {
                    var DLLFileName = (actionList[0].ActionObject as FSAutomator.Backend.Automators.ExternalAutomator).DLLName;
                    var DLLPath = (actionList[0].ActionObject as FSAutomator.Backend.Automators.ExternalAutomator).DLLPath;
                    var SavedDLLFileName = Path.Combine(destinationPath, DLLFileName);
                    ExportDLL(DLLPath, SavedDLLFileName);
                }
                else   //this is a JSON, maybe within a pack, maybe standalone
                {


                    List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);

                    if (dllFilesInAction.Any()) //is a pack
                    {
                        var packageName = automationFile.PackageName;
                        var automationFileName = automationFile.FileName;

                        bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction,Path.Combine(@"Automations",packageName));

                        if (allDLLsExist)
                        {
                            var json = Utils.GetJSONTextFromAutomationList(actionList, packageName);
                            ExportPack(fileName, packageName, automationFileName, automationFile, json);

                        }
                    }
                    else //standalone json
                    {
                        var jsonFileName = Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(fileName) + ".json");
                        var json = Utils.GetJSONTextFromAutomationList(actionList);
                        ExportJson(jsonFileName, json);
                    }

                }

                return true;

            }

            return false;

        }

        private void ExportPack(string filename, string packageName, string automationFileName, AutomationFile l_SAutomationFilesList, string json)
        {
            var tempDirPath = Path.Combine(@"Temp", filename);
            var JSONPath = Path.Combine(tempDirPath, l_SAutomationFilesList.FileName);
            var newJSONPath = Path.Combine(tempDirPath, filename + ".json");
            Directory.CreateDirectory(tempDirPath);
            Utils.CopyFullDirectory(Path.Combine("Automations", packageName).ToString(), tempDirPath);
            Utils.DeleteFilesFromDirectoryWithExtension(tempDirPath, "bak");
            File.Delete(JSONPath);
            File.WriteAllText(newJSONPath, json);
            var exportsPath = Path.Combine(Directory.GetParent(filename).ToString(), @"Exports");
            var zipPath = Path.Combine(exportsPath, filename + ".zip");



            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            ZipFile.CreateFromDirectory(tempDirPath, zipPath, CompressionLevel.NoCompression, false);
            Directory.Delete(tempDirPath, true);
        }

        private void ExportDLL(string DLLName, string saveDLL)
        {
            File.Copy(DLLName, saveDLL);
        }

        private void ExportJson(string filename, string json)
        {
            
            File.WriteAllText(filename, json);
        }



    }
}
