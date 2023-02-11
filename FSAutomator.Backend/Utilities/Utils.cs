using FSAutomator.Backend.Entities;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FSAutomator.Backend.Actions;
using System.Text;
using Microsoft.FlightSimulator.SimConnect;

namespace FSAutomator.Backend.Utilities
{

    public static class Utils
    {
        public static bool IsNumericDouble(string previousVariableValue)
        {
            return double.TryParse(previousVariableValue, out _);
        }

        public static Type GetType(string type)
        {
            Type t = Type.GetType(type);
            return t;
        }

        public static void CopyFullDirectory(string source, string target)
        {
            /*
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, target));
            }
            */

            foreach (string fileTempPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                var newPath = fileTempPath.Replace(source, target);

                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                File.Copy(fileTempPath, newPath, true);

            }
        }

        public static List<string> GetDLLFilesInJSONActionList(ObservableCollection<FSAutomatorAction> actionList)
        {
            var a =  actionList.Where(x => x.ActionObject.GetType().GetProperty("DLLName")?.GetValue(x.ActionObject) != null).Select(y => Path.Combine(Directory.GetParent(y.AutomationFile.FilePath).FullName, y.ActionObject.GetType().GetProperty("DLLName").GetValue(y.ActionObject).ToString())).Distinct().ToList();
            return a;
        }

        public static bool CheckIfAllDLLsInActionFileExist(List<string> dllFilesInAction, string packDirName="")
        {
            var allDLLsExist = true;

            foreach (string fullDLLPath in dllFilesInAction)
            {

                if (!File.Exists(Path.Combine(fullDLLPath)))
                {
                    allDLLsExist = false;
                }
            }

            return allDLLsExist;
        }

        public static ObservableCollection<FSAutomatorAction> GetAutomationsObjectList(AutomationFile fileToLoad)
        {
            var filePath = fileToLoad.FilePath;

            var json = File.ReadAllText(filePath);

            var jsonObject = JObject.Parse(json);

            var actionsNode = jsonObject["Actions"].ToArray();

            var actionsList = CreateActionList(fileToLoad, actionsNode);

            return actionsList;

        }

        private static ObservableCollection<FSAutomatorAction> CreateActionList(AutomationFile fileToLoad, JToken[] actions)
        {
            ObservableCollection<FSAutomatorAction> actionsList = new ObservableCollection<FSAutomatorAction>();

            foreach (JToken token in actions)
            {
                var actionName = token["Name"].ToString();
                var uniqueID = Guid.NewGuid().ToString();
                bool isAuxiliary = false;
                bool stopOnError = false;

                if (token["UniqueID"] != null)
                {
                    uniqueID = token["UniqueID"].ToString() != "" ? token["UniqueID"].ToString() : uniqueID;
                }

                if (token["IsAuxiliary"] != null)
                {
                    isAuxiliary = token["IsAuxiliary"].ToString().ToLower() == "true" ? true : false;
                }
                
                if (token["StopOnError"] != null)
                {
                    stopOnError = token["StopOnError"].ToString().ToLower() == "true" ? true : false;
                }
                var actionParameters = token["Parameters"].ToString();

                Type actionType = Type.GetType(String.Format("FSAutomator.Backend.Actions.{0}", actionName));
                var actionObject = Activator.CreateInstance(actionType);

                actionObject = JsonConvert.DeserializeObject(actionParameters, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                
                /*
                if (actionName == "ExecuteCodeFromDLL")
                {
                    (actionObject as ExecuteCodeFromDLL).DLLPath = Path.Combine(Directory.GetParent(fileToLoad.FilePath).FullName, (actionObject as ExecuteCodeFromDLL).DLLName);
                }
                */

                var action = new FSAutomatorAction(actionName, uniqueID, "Pending", actionParameters, actionObject, isAuxiliary, stopOnError, fileToLoad);

                actionsList.Add(action);
            }
            return actionsList;
        }

        public static List<AutomationFile> GetAutomationFilesList()
        {
            var automationFiles = Directory.GetFiles(@"Automations", "*.json", SearchOption.TopDirectoryOnly).ToList();
            automationFiles.AddRange(Directory.GetFiles(@"Automations", "*.dll", SearchOption.TopDirectoryOnly).ToList());

            var automationsToLoad = automationFiles.Select(automationFilePath => new AutomationFile(Path.GetFileName(automationFilePath), "", String.Format("{0} [{1}]", Path.GetFileNameWithoutExtension(automationFilePath), Path.GetExtension(automationFilePath)), automationFilePath, Directory.GetParent(automationFilePath).FullName)).ToList();

            var automationPackPaths = Directory.GetDirectories(@"Automations");

            foreach (string packPath in automationPackPaths)
            {
                var filePath = Directory.GetFiles(packPath,"*.json",SearchOption.TopDirectoryOnly).FirstOrDefault(); // note controlar si hi ha més d'un no es carrega
                var fileName = Path.GetFileName(filePath);
                var packageName = Path.GetFileNameWithoutExtension(fileName);
                var visibleName = String.Format("{0} [{1}]", packageName, "json, pack");
                var basePath = Directory.GetParent(filePath).FullName;

                var jsonAutomationFile = new AutomationFile(fileName, packageName, visibleName, filePath, basePath);

                var actionList = Utils.GetAutomationsObjectList(jsonAutomationFile);

                var dllFilesAsExternalAutomatorAutomationFileList = actionList.Where(x => x.Name == "ExecuteCodeFromDLL")
                    .Where(y => (y.ActionObject as ExecuteCodeFromDLL).IncludeAsExternalAutomator == true)
                    .Select(z => new AutomationFile(
                        (z.ActionObject as ExecuteCodeFromDLL).DLLName,
                        packageName,
                        String.Format("{0} [{1}{2}]", Path.GetFileNameWithoutExtension((z.ActionObject as ExecuteCodeFromDLL).DLLName), "dll, ", packageName),
                        Path.Combine("Automations",Directory.GetParent(filePath).Name, (z.ActionObject as ExecuteCodeFromDLL).DLLName),
                        Directory.GetParent(Path.Combine("Automations", Directory.GetParent(filePath).Name, (z.ActionObject as ExecuteCodeFromDLL).DLLName)).FullName
                        )).ToList();

                automationsToLoad.Add(jsonAutomationFile);
                automationsToLoad.AddRange(dllFilesAsExternalAutomatorAutomationFileList);
            }
            return automationsToLoad;
        }

        public static void DeleteFilesFromDirectoryWithExtension(string path, string extension)
        {
            var filePatternToDelete = "*." + extension;
            var files = Directory.GetFiles(path, filePatternToDelete);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public static string GetJSONTextFromAutomationList(ObservableCollection<FSAutomatorAction> automationList, string packageName = "")
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("Actions");
                writer.WriteStartArray();

                

                foreach (FSAutomatorAction action in automationList)
                {

                    writer.WriteStartObject();

                    writer.WritePropertyName("Name");
                    writer.WriteValue(action.Name);
                    writer.WritePropertyName("UniqueID");
                    writer.WriteValue(action.UniqueID);

                    writer.WritePropertyName("Parameters");
                    writer.WriteStartObject();

                    var propertyNames = action.ActionObject.GetType().GetProperties().Select(x => x.Name).ToList();

                    foreach (var name in propertyNames)
                    {
                        var value = action.ActionObject.GetType().GetProperty(name).GetValue(action.ActionObject);
                        if ((action.Name != "ExecuteCodeFromDLL") || ((action.Name == "ExecuteCodeFromDLL") && (name != "DLLPath")))
                        {
                            writer.WritePropertyName(name);
                            writer.WriteValue(value);
                        }
                    }

                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();

                var finalJSON = sb.ToString();
                return finalJSON;
            }
        }

        public static string GetValueToOperateOnFromTag(object sender, SimConnect connection, string itemIdentificator)
        {
            if (!(itemIdentificator.Contains("%") || itemIdentificator.StartsWith("%")))
            {
                return itemIdentificator;
            }

            var itemId = itemIdentificator.Split('%')[1];
            var itemArg = itemIdentificator.Split('%')[2];

            var valueToOperateOn = itemId;

            if (itemId == "PrevValue")
            {
                valueToOperateOn = (string)sender.GetType().GetField("lastOperationValue").GetValue(sender);
            }
            else if (itemId == "FM")
            {
                FlightModel fm = (FlightModel)sender.GetType().GetField("flightModel").GetValue(sender);
                var property = fm.ReferenceSpeeds.GetType().GetProperty(itemArg);

                if (property != null)
                {
                    valueToOperateOn = (string)property.GetValue(fm.ReferenceSpeeds);
                }
                else
                {
                    valueToOperateOn = String.Format("{0} not found in the flight model", itemArg);
                }
            }
            else if (itemId == "AutomationId")
            {
                var actionList = (ObservableCollection<FSAutomatorAction>)sender.GetType().GetField("ActionList").GetValue(sender);

                valueToOperateOn = actionList.Where(action => action.UniqueID == itemArg).Select(x => x.Result.ComputedResult).First().ToString();
            }
            else if (itemId == "MemoryRegister")
            {
                var memoryRegisters = (Dictionary<string, string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);

                valueToOperateOn = memoryRegisters.Where(r => r.Key == itemArg).Select(x => x.Value).First().ToString();
            }
            else if (itemId == "Variable")
            {
                valueToOperateOn = new GetVariable(itemArg).ExecuteAction(sender, connection).ComputedResult;
                //note pensar de posar a default a null el automationFile d'aquelles variables que no el necessitin.
            }

            return valueToOperateOn;
        }
    }
}

