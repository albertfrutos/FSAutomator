using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.AutomationImportersAndExporters;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.BackEnd.Validators;
using FSAutomator.SimConnectInterface;
using Microsoft.FlightSimulator.SimConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FSAutomator.Backend.Entities.FSAutomatorAction;
using static FSAutomator.SimConnectInterface.Entities;

namespace FSAutomator.Backend
{
    public class BackendMain
    {
        private ISimConnectBridge Connection;

        private EventWaitHandle _simConnectEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public Automator automator = null;

        private Thread _simConnectReceiveThread = null;

        public GeneralStatus status = GeneralStatus.GetInstance;

        public ApplicationConfig config = ApplicationConfig.GetInstance;

        public BackendMain()
        {
            this.Initialize();
        }

        public void Execute()
        {

            automator.ExecuteActionList();
        }

        public InternalMessage SaveAutomation(AutomationFile automation, string newFileName)
        {

            if (!(newFileName.Length > 0))
            {
                return new InternalMessage("Please enter an automation name.", false);
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);

            var automationFilePath = Path.Combine(config.AutomationsFolder, automation.PackageName, fileNameWithoutExtension + ".json");
            var isDLLAutomation = automator.ActionList.Where(x => x.Name == "DLLAutomation").Any();

            if (isDLLAutomation)
            {
                return new InternalMessage("Saving DLL automations is not supported", false);
            }

            if (!automator.ActionList.Any())
            {
                return new InternalMessage("No actions to save", false);
            }

            var json = Utils.GetJSONTextFromAutomationList(automator.ActionList);

            File.WriteAllText(automationFilePath, json);

            return new InternalMessage("Automation saved successfully", false);
        }

        public void LoadActions(AutomationFile fileToLoad)
        {
            ClearAutomationList();

            if (fileToLoad.FileName.EndsWith(".json"))
            {
                LoadJSONActions(fileToLoad);
            }
            else if (fileToLoad.FileName.EndsWith(".dll"))
            {
                LoadDLLActions(fileToLoad);
            }

            ValidateActions();
        }

        private void LoadDLLActions(AutomationFile fileToLoad)
        {
            var fileToLoadPath = Path.Combine(config.AutomationsFolder, fileToLoad.PackageName, fileToLoad.FileName);
            var uniqueID = Guid.NewGuid().ToString();

            AddAction(new FSAutomatorAction("DLLAutomation", uniqueID, ActionStatus.Pending, fileToLoadPath, false, true, fileToLoad));
        }

        public List<string> ValidateActions()
        {
            status.ValidationIssues = ActionJSONValidator.ValidateActions(automator.ActionList.ToArray());
            return status.ValidationIssues;
        }

        public List<string> GetValidationIssuesList()
        {
            return status.ValidationIssues;
        }

        private void LoadJSONActions(AutomationFile fileToLoad)
        {
            var actionList = Utils.GetActionsList(fileToLoad, true);

            AddActions(actionList);
           
            return;
        }

        private void AddActions(ObservableCollection<FSAutomatorAction> actionList)
        {
            foreach (FSAutomatorAction action in actionList)
            {
                AddAction(action);
            }
        }

        public void AddAction(FSAutomatorAction action)
        {
            if (!action.IsAuxiliary)
            {
                automator.ActionList.Add(action);
            }
            else
            {
                automator.AuxiliaryActionList.Add(action);
            }

            automator.RebuildActionListIndices();
        }

        public void AddJSONActionAfterPosition(int position, string actionJSON, AutomationFile automationFile)
        {
            var jsonObject = JObject.Parse(actionJSON);
            var actionName = jsonObject["Name"].ToString();
            var uniqueID = jsonObject["UniqueID"].ToString();
            var stopOnError = (bool)jsonObject["StopOnError"];
            var actionParameters = jsonObject["Parameters"].ToString();

            if (!Utils.CheckIfActionExists(actionName))
            {
                var message = new InternalMessage($"Action name ({actionName}) does not exist. Did you select an action?", true);
                status.ReportStatus(message);
                return;
            }

            FSAutomatorAction action = new FSAutomatorAction(actionName, uniqueID, ActionStatus.Pending, actionParameters, false, stopOnError, automationFile);

            automator.ActionList.Insert(position + 1, action);

            automator.RebuildActionListIndices();
        }

        public int MoveActionDown(int selectedIndex)
        {
            if (selectedIndex == automator.ActionList.Count - 1)
            {
                return selectedIndex;
            }

            FSAutomatorAction temp = automator.ActionList[selectedIndex];
            automator.ActionList[selectedIndex] = automator.ActionList[selectedIndex + 1];
            automator.ActionList[selectedIndex + 1] = temp;

            automator.RebuildActionListIndices();

            return selectedIndex + 1;
        }

        public void Initialize()
        {
            this.Connection = new SimConnectBridge();
            this.automator = new Automator(this.Connection);

            status.ReportStatusEvent += ProcessErrorEvent;
        }

        private void ProcessErrorEvent(object sender, InternalMessage internalMessage)
        {
            if (internalMessage.Type == InternalMessage.MsgType.Critical)
            {
                status.GeneralErrorHasOcurred = true;
                Disconnect();
            }
        }

        public int MoveActionUp(int selectedIndex)
        {
            if (selectedIndex == 0 || selectedIndex == -1)
            {
                return selectedIndex;
            }

            FSAutomatorAction temp = automator.ActionList[selectedIndex];
            automator.ActionList[selectedIndex] = automator.ActionList[selectedIndex - 1];
            automator.ActionList[selectedIndex - 1] = temp;

            automator.RebuildActionListIndices();

            return selectedIndex - 1;
        }

        public void RemoveAction(int index)
        {
            if (automator.ActionList.Count > 0 && index >= 0)
            {
                automator.ActionList.Remove(automator.ActionList[index]);
                automator.RebuildActionListIndices();
            }
        }


        public InternalMessage ExportAutomation(string filename, AutomationFile l_SAutomationFilesList)
        {
            var exportStatus = new Exporters().ExportAutomation(filename, this.automator.ActionList, l_SAutomationFilesList);
            if (exportStatus.Type == InternalMessage.MsgType.Error)
            {
                ClearAutomationList();
            }
            return exportStatus;
        }

        public void ClearAutomationList()
        {
            automator.ActionList.Clear();
            automator.AuxiliaryActionList.Clear();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnect");
            Connection.Disconnect();
            status.IsConnectedToSim = Connection.IsConnected();
        }

        public void Connect()
        {
            try
            {
                Connection.Connect();
                Connection.ConnectionStatusChangeEvent += HandleConnectionStatusChange;
            }
            catch (COMException ex)
            {
                Trace.WriteLine("Connection to simulator failed: " + ex.Message);
            }
        }

        private void HandleConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
        {
            status.IsConnectedToSim = Connection.IsConnected();

            switch (e.ConnectionStatus)
            {
                case ConnectionStatus.Open:
                    status.GeneralErrorHasOcurred = false;
                    break;
                case ConnectionStatus.Exception:
                    status.ReportStatus(new InternalMessage("An exception ocurred with the connection to the sim and the automation will be stopped: " + e.Message, true, true));
                    break;
                case ConnectionStatus.Failed:
                    status.ReportStatus(new InternalMessage("Connection to simulator failed. Automation will be aborted: " + e.Message, true, true));
                    break;
            }
        }

        public void ImportAutomationFromFilePath(string filepath)
        {
            new Importers().ImportAutomationFromFilePath(filepath);
        }

        public ObservableCollection<FSAutomatorAction> GetActionList()
        {
            return automator.ActionList;
        }
    }
}
