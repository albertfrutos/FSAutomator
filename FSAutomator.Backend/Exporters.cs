﻿using FSAutomator.Backend.Actions;
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
        internal bool ExportAutomation(string fileName, string destinationPath, ObservableCollection<FSAutomatorAction> l_ActionList, AutomationFile l_SAutomationFilesList)
        {
            var exportPath = @"Exports";
            if (l_ActionList is not null && l_ActionList.Count() > 0)
            {
                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }

                if (l_ActionList[0].Name == "DLLAutomation")
                {
                    var DLLFileName = (l_ActionList[0].ActionObject as FSAutomator.Backend.Automators.ExternalAutomator).DLLName;
                    var DLLPath = (l_ActionList[0].ActionObject as FSAutomator.Backend.Automators.ExternalAutomator).DLLPath;
                    var SavedDLLFileName = Path.Combine(destinationPath, DLLFileName);
                    ExportDLL(DLLPath, SavedDLLFileName);
                }
                else   //this is a JSON, maybe within a pack, maybe standalone
                {


                    List<object> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(l_ActionList);

                    if (dllFilesInAction.Any()) //is a pack
                    {
                        var packageName = l_SAutomationFilesList.PackageName;
                        var automationFileName = l_SAutomationFilesList.FileName;

                        bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction,Path.Combine(@"Automations",packageName));

                        if (allDLLsExist)
                        {
                            var json = Utils.GetJSONTextFromAutomationList(l_ActionList, packageName);
                            ExportPack(fileName, packageName, automationFileName, l_SAutomationFilesList, json);

                        }
                    }
                    else //standalone json
                    {
                        var jsonFileName = Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(fileName) + ".json");
                        var json = Utils.GetJSONTextFromAutomationList(l_ActionList);
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
