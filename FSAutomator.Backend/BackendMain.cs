using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.AutomationImportersAndExporters;
using FSAutomator.BackEnd.Configuration;
using FSAutomator.BackEnd.Validators;
using Microsoft.FlightSimulator.SimConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FSAutomator.Backend.Entities.FSAutomatorAction;

namespace FSAutomator.Backend
{
    public class BackendMain
    {
        private SimConnect m_SimConnect = null;

        private EventWaitHandle _simConnectEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public Automator automator = new Automator();

        private Thread _simConnectReceiveThread = null;

        public GeneralStatus status = GeneralStatus.GetInstance;

        public ApplicationConfig config = ApplicationConfig.GetInstance;

        public BackendMain()
        {
        }

        public void Execute()
        {

            automator.ExecuteActionList();
        }

        public SimConnect Connection
        {
            get
            {
                return m_SimConnect;
            }
            set { }
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

            //var externalAutomatorObject = new ExternalAutomator(fileToLoad.FileName, fileToLoadPath);
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

            var actionList = Utils.GetActionsList(fileToLoad);

            AddActions(actionList);
            //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);


            //var actionList = Utils.GetAutomationsObjectList(fileToLoad);
            /*
            if (actionList is null)
            {
                var exMessage = String.Format("There was a problem while processing the action list for {0}", fileToLoad.FileName);
                status.ReportStatus(new InternalMessage(exMessage, true));
                return;
            }
            
            
            foreach (FSAutomatorAction action in actionList)
            {
                var finalAction = ApplyActionModifications(fileToLoad, action);

                AddAction(finalAction);
            }
            */

            return;
        }



        
        /*
        private static FSAutomatorAction ApplyActionModifications(AutomationFile fileToLoad, FSAutomatorAction action)
        {
            if (action.Name == "ExecuteCodeFromDLL")
            {
                (action.ActionObject as ExecuteCodeFromDLL).PackFolder = fileToLoad.PackageName;
            }
            

            
            if (action.Name == "ExpectVariableValue")
            {
                (action.ActionObject as ExpectVariableValue).getVariable = new GetVariable();
            }


            return action;
        }
        */

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

            var actionParameters = jsonObject["Parameters"].ToString();
            var uniqueID = Guid.NewGuid().ToString();
            var stopOnError = (bool)jsonObject["StopOnError"];

            if (Utils.CheckIfActionExists(actionName))
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

            if (m_SimConnect != null)
            {
                m_SimConnect.Dispose();
                m_SimConnect = null;
                status.IsConnectedToSim = false;
            }
        }

        public void Connect()
        {

            try
            {
                m_SimConnect = new SimConnect("Simconnect - FSAutomator", IntPtr.Zero, 0, _simConnectEventHandle, 0);

                m_SimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
                m_SimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Simconnect_OnRecvQuit);
                m_SimConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);

                StartMessageReceiveThreadHandler();
            }
            catch (COMException ex)
            {
                Trace.WriteLine("Connection to simulator failed: " + ex.Message);
            }



        }

        private void StartMessageReceiveThreadHandler()
        {
            _simConnectReceiveThread = new Thread(new ThreadStart(SimConnect_MessageReceiveThreadHandler));
            _simConnectReceiveThread.IsBackground = true;
            _simConnectReceiveThread.Start();

        }
        private void SimConnect_MessageReceiveThreadHandler()
        {
            while (true)
            {
                _simConnectEventHandle.WaitOne();

                try
                {
                    m_SimConnect?.ReceiveMessage();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.automator.connection = this.m_SimConnect;
            status.IsConnectedToSim = true;
            status.GeneralErrorHasOcurred = false;
        }

        private void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("An exception occurred Simconnect_OnRecvException: {0}", data.dwException.ToString());
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Disconnect();

            // This causes a trigger of a general (critical error). This means that the automation will be stopped.
            status.ReportStatus(new InternalMessage("An exception ocurred with the connection to the sim and the automation will be stopped: " + eException.ToString(), true, true));
        }

        private void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            status.IsConnectedToSim = false;
            Disconnect();
            Console.WriteLine("Simulator has exited. Closing connection and exiting Simulator module");
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

    public class Root
    {
        public List<FSAutomatorAction> Actions { get; set; }
    }

    public class Action
    {
        public string Name { get; set; }
        public object Parameters { get; set; }
    }
}
