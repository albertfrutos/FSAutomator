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
    internal class Importers
    {
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
            var tempDirPath = Path.Combine(@"Temp", tempDirName);

            ZipFile.ExtractToDirectory(ZIPPath, tempDirPath);

            var jsonFileName = Directory.GetFiles(tempDirPath, "*.json");

            if (jsonFileName.Count() > 1)
            {
                Trace.WriteLine("Only packages with one JSON file are supported");
            }
            else
            {
                var JSONPath = jsonFileName[0];

                var automationFile = new AutomationFile(Path.GetFileName(JSONPath), Path.GetFileNameWithoutExtension(JSONPath), "", JSONPath, "");

                var actionList = Utils.GetAutomationsObjectList(automationFile);

                List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);

                bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction, tempDirPath);

                if (allDLLsExist)
                {
                    var automationsTargetDir = Path.Combine(@"Automations", tempDirName);
                    Utils.CopyFullDirectory(tempDirPath, automationsTargetDir);
                }
            }

            Directory.Delete(tempDirPath, true);
        }


        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        private void ImportDLLAutomation(string filepath)
        {

            var dllFileName = Path.GetFileName(filepath);

            var destinationDLLPath = Path.Combine(@"Automations", dllFileName);

            if (!File.Exists(destinationDLLPath))
            {
                File.Copy(filepath, destinationDLLPath);
            }
        }

        private void ImportJSONAutomation(string JSONPath)
        {
            var JSONFileName = Path.GetFileName(JSONPath);

            var automationFile = new AutomationFile(JSONFileName, "", "", JSONPath, "");

            var actionList = Utils.GetAutomationsObjectList(automationFile);

            List<string> dllFilesInAction = Utils.GetDLLFilesInJSONActionList(actionList);

            //JSON has references to DLL files, so it's considered like a pack
            if (dllFilesInAction.Any())
            {

                bool allDLLsExist = Utils.CheckIfAllDLLsInActionFileExist(dllFilesInAction);

                if (allDLLsExist)
                {
                    File.Copy(JSONPath, Path.Combine(@"Automations", JSONFileName));

                    foreach (string dllFilePath in dllFilesInAction)
                    {
                        var dllFileName = Path.GetFileName(dllFilePath);

                        var destinationDLLPath = Path.Combine(@"Automations", dllFileName);

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
                File.Copy(JSONPath, Path.Combine(@"Automations", fileName));

            }

        }

    }
}
