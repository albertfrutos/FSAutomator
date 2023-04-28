using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Configuration;
using FSAutomator.SimConnectInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.ObjectModel;
using System.Text;
using static FSAutomator.Backend.Entities.FSAutomatorAction;

namespace FSAutomator.Backend.Utilities
{

    public static class Utils
    {
        public static ApplicationConfig Config = ApplicationConfig.GetInstance;


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
            var a = actionList.Where(x => x.ActionObject.GetType().GetProperty("DLLName")?.GetValue(x.ActionObject) != null).Select(y => Path.Combine(Directory.GetParent(y.AutomationFile.FilePath).FullName, y.ActionObject.GetType().GetProperty("DLLName").GetValue(y.ActionObject).ToString())).Distinct().ToList();
            return a;
        }

        public static bool CheckIfAllDLLsInActionFileExist(List<string> dllFilesInAction)
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

        public static bool CheckIfActionExists(string actionName)
        {
            var availableActions = GetExistingActions();

            return availableActions.Contains(actionName);
        }

        public static List<string> GetExistingActions()
        {
            var actions = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(t => t.GetTypes())
               .Where(t => t.IsClass && t.IsNested == false && t.Namespace == "FSAutomator.Backend.Actions")
               .Select(T => T.Name)
               .ToList();

            return actions;
        }

        /*
        private static ObservableCollection<FSAutomatorAction> CreateActionList(AutomationFile fileToLoad, JToken[] actions)
        {
            ObservableCollection<FSAutomatorAction> actionsList = new ObservableCollection<FSAutomatorAction>();


            foreach (JToken token in actions)
            {
                var actionName = token["Name"].ToString();

                if (!CheckIfActionExists(actionName))
                {
                    var exMessage = new InternalMessage($"The action {actionName} is not supported.", true);
                    GeneralStatus.GetInstance.ReportStatus(exMessage);
                    return null;
                }

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
                var actionParameters = token["Parameters"] is null ? "" : token["Parameters"].ToString();

                var action = new FSAutomatorAction(actionName, uniqueID, ActionStatus.Pending, actionParameters, isAuxiliary, stopOnError, fileToLoad);

                actionsList.Add(action);
            }
            return actionsList;
        }
        */

        public static ObservableCollection<FSAutomatorAction> GetActionsList(AutomationFile fileToLoad, bool validateAutomationJSON)
        {
            var json = File.ReadAllText(fileToLoad.FilePath);
            var actionList = new ObservableCollection<FSAutomatorAction>();
            var jsonObjectActions = JObject.Parse(json);


            if (validateAutomationJSON)
            {
                IList<string> JSONValidationErrors;

                var isValid = ValidateAutomationJSON(jsonObjectActions, out JSONValidationErrors);
                GeneralStatus.GetInstance.JSONSchemaValidationIssues = JSONValidationErrors.ToList();

                if (!isValid)
                {
                    GeneralStatus.GetInstance.ReportStatus(new InternalMessage($"JSON from {fileToLoad.FileName} is not valid according to the schema", false, false));
                }
            }

            var actionsNode = jsonObjectActions["Actions"].ToArray();

            if (!actionsNode.Any())
            {
                var exMessage = String.Format("No actions defined in JSON file {0}", fileToLoad.FileName);
                GeneralStatus.GetInstance.ReportStatus(new InternalMessage(exMessage, true));
                return null;
            }

            foreach (JToken action in actionsNode)
            {
                var actionName = action["Name"]?.ToString();

                if (!Utils.CheckIfActionExists(actionName))
                {
                    var exMessage = new InternalMessage($"The action {actionName} is not supported.", true);
                    GeneralStatus.GetInstance.ReportStatus(exMessage);
                    return null;
                }

                var actionUniqueID = String.IsNullOrEmpty(action["UniqueID"]?.ToString().Trim()) ? Guid.NewGuid().ToString() : action["UniqueID"]?.ToString();
                var actionParameters = action["Parameters"]?.ToString();
                var actionIsAuxiliary = Convert.ToBoolean(action["IsAuxiliary"]?.ToString());
                var actionStopOnError = Convert.ToBoolean(action["StopOnError"]?.ToString());
                var parallelLaunch = Convert.ToBoolean(action["ParallelLaunch"]?.ToString());

                actionList.Add(new FSAutomatorAction(
                    actionName,
                    actionUniqueID,
                    ActionStatus.Pending,
                    actionParameters,
                    actionIsAuxiliary,
                    actionStopOnError,
                    parallelLaunch,
                    fileToLoad
                    ));

            }

            return actionList;

        }

        public static bool ValidateAutomationJSON(JObject jsonObjectActions, out IList<string> JSONValidationErrors)
        {
            var jsonSchemaText = File.ReadAllText(Config.SchemaFile);
            JSchema schema = JSchema.Parse(jsonSchemaText);
            return jsonObjectActions.IsValid(schema, out JSONValidationErrors);
        }

        public static List<AutomationFile> GetAutomationFilesList()
        {
            var automationFiles = Directory.GetFiles(Config.AutomationsFolder, "*.json", SearchOption.TopDirectoryOnly).ToList();
            automationFiles.AddRange(Directory.GetFiles(Config.AutomationsFolder, "*.dll", SearchOption.TopDirectoryOnly).ToList());

            var automationsToLoad = automationFiles.Select(automationFilePath => new AutomationFile(Path.GetFileName(automationFilePath), "", String.Format("{0} [{1}]", Path.GetFileNameWithoutExtension(automationFilePath), Path.GetExtension(automationFilePath)), automationFilePath, Directory.GetParent(automationFilePath).FullName, false)).ToList();

            var automationPackPaths = Directory.GetDirectories(Config.AutomationsFolder);

            foreach (string packPath in automationPackPaths)
            {
                var filePath = Directory.GetFiles(packPath, "*.json", SearchOption.TopDirectoryOnly).FirstOrDefault();

                var fileName = Path.GetFileName(filePath);
                var packageName = Path.GetFileNameWithoutExtension(fileName);
                var visibleName = String.Format("{0} [{1}]", packageName, "json, pack");
                var basePath = Directory.GetParent(filePath).FullName;

                var jsonAutomationFile = new AutomationFile(fileName, packageName, visibleName, filePath, basePath, true);

                var actionList = Utils.GetActionsList(jsonAutomationFile, false);

                if (actionList is null)
                {
                    continue;
                }

                var dllFilesAsExternalAutomatorAutomationFileList = actionList.Where(x => x.Name == "ExecuteCodeFromDLL")
                    .Where(y => (y.ActionObject as ExecuteCodeFromDLL).IncludeAsExternalAutomator == true)
                    .Select(z => new AutomationFile(
                        (z.ActionObject as ExecuteCodeFromDLL).DLLName,
                        "",
                        String.Format("{0} [{1}{2}]", Path.GetFileNameWithoutExtension((z.ActionObject as ExecuteCodeFromDLL).DLLName), "dll, ", packageName),
                        Path.Combine(Config.AutomationsFolder, Directory.GetParent(filePath).Name, (z.ActionObject as ExecuteCodeFromDLL).DLLName),
                        Directory.GetParent(Path.Combine(Config.AutomationsFolder, Directory.GetParent(filePath).Name, (z.ActionObject as ExecuteCodeFromDLL).DLLName)).FullName
                        )).ToList();

                automationsToLoad.Add(jsonAutomationFile);
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
                    writer.WritePropertyName("StopOnError");
                    writer.WriteValue(action.StopOnError);
                    writer.WritePropertyName("ParallelLaunch");
                    writer.WriteValue(action.ParallelLaunch);

                    if (action.ActionObject != null)
                    {
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
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();

                var finalJSON = sb.ToString();
                return finalJSON;
            }
        }

        public static string GetValueToOperateOnFromTag(object sender, ISimConnectBridge connection, string itemIdentificator)
        {
            if (!itemIdentificator.StartsWith("%"))
            {
                return itemIdentificator;
            }

            var itemId = itemIdentificator.Split('%')[1];
            var itemArg = itemIdentificator.Split('%')[2];

            var valueToOperateOn = itemIdentificator;

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

                valueToOperateOn = actionList.Where(action => action.UniqueID == itemArg).Select(x => x.Result.ComputedResult).FirstOrDefault().ToString();
            }
            else if (itemId == "MemoryRegister")
            {
                var memoryRegisters = (Dictionary<string, string>)sender.GetType().GetField("MemoryRegisters").GetValue(sender);

                valueToOperateOn = memoryRegisters.Where(r => r.Key == itemArg).Select(x => x.Value).FirstOrDefault().ToString();
            }
            else if (itemId == "Variable")
            {
                valueToOperateOn = new GetVariable(itemArg).ExecuteAction(sender, connection).ComputedResult;
            }

            return valueToOperateOn;
        }

        public static string RecalculateColorForKML(string color)
        {
            var R = color.Substring(0, 2);
            var G = color.Substring(2, 2);
            var B = color.Substring(4, 2);

            var kmlCodifiedColor = $"{B}{G}{R}";

            return kmlCodifiedColor;
        }

        public static bool TryParse<T>(string text, out T value)
        {
            value = default(T);
            try
            {
                value = (T)Convert.ChangeType(text, typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

