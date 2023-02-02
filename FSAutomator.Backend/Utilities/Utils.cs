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
            foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, target));
            }


            foreach (string newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source, target), true);

            }
        }

        public static List<object> GetDLLFilesInJSONActionList(ObservableCollection<FSAutomatorAction> actionList)
        {
            return actionList.Where(x => x.ActionObject.GetType().GetProperty("DLLName")?.GetValue(x.ActionObject) != null).Select(y => y.ActionObject.GetType().GetProperty("DLLName").GetValue(y.ActionObject)).Distinct().ToList();
        }

        public static bool CheckIfAllDLLsInActionFileExist(List<object> dllFilesInAction)
        {
            var allDLLsExist = true;

            foreach (string fullDLLPath in dllFilesInAction)
            {

                if (!File.Exists(fullDLLPath))
                {
                    allDLLsExist = false;
                }
            }

            return allDLLsExist;
        }

        public static ObservableCollection<FSAutomatorAction> GetAutomationsObjectList(string automationPath)
        {

            var json = File.ReadAllText(automationPath);

            var jsonObject = JObject.Parse(json);

            var actionsNode = jsonObject["Actions"].ToArray();

            var actionsList = CreateActionList(automationPath, actionsNode);

            return actionsList;

        }

        private static ObservableCollection<FSAutomatorAction> CreateActionList(string automationPath, JToken[] actions)
        {
            ObservableCollection<FSAutomatorAction> actionsList = new ObservableCollection<FSAutomatorAction>();

            foreach (JToken token in actions)
            {
                var actionName = token["Name"].ToString();
                var uniqueID = Guid.NewGuid().ToString();
                bool isAuxiliary = false;
                if (token["UniqueID"] != null)
                {
                    uniqueID = token["UniqueID"].ToString() != "" ? token["UniqueID"].ToString() : uniqueID;
                }

                if (token["IsAuxiliary"] != null)
                {
                    isAuxiliary = token["IsAuxiliary"].ToString().ToLower() == "true" ? true : false;
                }
                var actionParameters = token["Parameters"].ToString();

                Type actionType = Type.GetType(String.Format("FSAutomator.Backend.Actions.{0}", actionName));
                var actionObject = Activator.CreateInstance(actionType);

                actionObject = JsonConvert.DeserializeObject(actionParameters, actionType);
                /*
                if (actionName == "ExecuteCodeFromDLL")
                {
                    (actionObject as ExecuteCodeFromDLL).DLLPackageFolder = Directory.GetParent(automationPath).Name;
                }
                */
                var action = new FSAutomatorAction(actionName, uniqueID, "Pending", actionParameters, actionObject, isAuxiliary);

                actionsList.Add(action);
            }
            return actionsList;
        }

        public static List<AutomationFile> GetAutomationFilesList()
        {
            var automationFiles = Directory.GetFiles(@"Automations", "*.json", SearchOption.TopDirectoryOnly).ToList();
            automationFiles.AddRange(Directory.GetFiles(@"Automations", "*.dll", SearchOption.TopDirectoryOnly).ToList());

            var automationsToLoad = automationFiles.Select(x => new AutomationFile(Path.GetFileName(x), "", String.Format("{0} [{1}]", Path.GetFileNameWithoutExtension(x), Path.GetExtension(x)))).ToList();

            var automationPackPaths = Directory.GetDirectories(@"Automations");

            foreach (string packPath in automationPackPaths)
            {
                var jsonPackFileName = String.Format("{0}.json", new DirectoryInfo(packPath).Name);
                var jsonPackName = Path.GetFileNameWithoutExtension(jsonPackFileName);

                var actionList = Utils.GetAutomationsObjectList(Path.Combine("Automations", jsonPackName, jsonPackFileName));

                var dllFilesAsExternalAutomator = actionList.Where(x => x.Name == "ExecuteCodeFromDLL").Where(y => (y.ActionObject as ExecuteCodeFromDLL).IncludeAsExternalAutomator == true).Select(z => new AutomationFile((z.ActionObject as ExecuteCodeFromDLL).DLLName, jsonPackName, String.Format("{0} [{1}{2}]", Path.GetFileNameWithoutExtension((z.ActionObject as ExecuteCodeFromDLL).DLLName), "dll, ", jsonPackName))).ToList();

                automationsToLoad.Add(new AutomationFile(jsonPackFileName, jsonPackName, String.Format("{0} [{1}]", jsonPackName, "json, pack")));
                automationsToLoad.AddRange(dllFilesAsExternalAutomator);
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

                        writer.WritePropertyName(name);

                        writer.WriteValue(value);
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

                valueToOperateOn = actionList.Where(action => action.UniqueID == itemArg).Select(x => x.Result).First().ToString();
            }
            else if (itemId == "MemoryRegister")
            {
                var memoryRegisters = (Dictionary<string, string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);

                valueToOperateOn = memoryRegisters.Where(r => r.Key == itemArg).Select(x => x.Value).First().ToString();
            }
            else if (itemId == "Variable")
            {
                valueToOperateOn = new GetVariable(itemArg).ExecuteAction(sender, connection);
            }

            return valueToOperateOn;
        }
    }
}

