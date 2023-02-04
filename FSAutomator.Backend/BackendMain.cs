using FSAutomator.Backend.Actions;
using FSAutomator.Backend.Automators;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd;
using FSAutomator.BackEnd.Validators;
using Microsoft.FlightSimulator.SimConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FSAutomator.Backend
{
    public class BackendMain
    {
        private SimConnect m_SimConnect = null;

        public Automator automator = null;


        public BackendMain(ObservableCollection<FSAutomatorAction> ActionList)
        {
            this.automator = new Automator(ActionList);
        }

        public void Execute()
        {
            automator.Execute();
        }

        public SimConnect Connection
        {
            get {
                return m_SimConnect;
            }
            set { 
             

            }
        }

        public void SaveAutomation(AutomationFile automation)
        {
            var automationFilePath = Path.Combine("Automations", automation.PackageName, automation.FileName);
            var json = Utils.GetJSONTextFromAutomationList(automator.ActionList);
            File.WriteAllText(automationFilePath, json);
        }

        public void LoadActions(AutomationFile fileToLoad)
        {
            automator.ActionList.Clear();

            var fileToLoadPath = Path.Combine("Automations", fileToLoad.PackageName, fileToLoad.FileName);

            if (fileToLoad.FileName.EndsWith(".json"))
            {
                LoadJSONActions(fileToLoadPath); //"Automations\\bb\\bb.json"
            }
            else if (fileToLoad.FileName.EndsWith(".dll"))
            {
                LoadDLLActions(fileToLoad.FileName,fileToLoadPath);   //"Automations\\ExternalAutomationExample.dll"
            }
            ActionJSONValidator.ValidateActions(automator.ActionList.ToArray(), fileToLoadPath);
        }

        private void LoadDLLActions(string DLLFileName, string DLLFilePath)
        {
            //fer un getname i terure el nom
            var externalAutomatorObject = new ExternalAutomator(DLLFileName, DLLFilePath); //"Automations\\ExternalAutomationExample.dll"
            var uniqueID = Guid.NewGuid().ToString();
            automator.ActionList.Add(new FSAutomatorAction("DLLAutomation", uniqueID, "Pending", DLLFilePath, externalAutomatorObject,false,true));
        }

        public void ValidateActions(string JSONFilePath)
        {
            ActionJSONValidator.ValidateActions(automator.ActionList.ToArray(), JSONFilePath);
        }

        public List<string> GetValidationIssuesList()
        {
            return automator.ActionList.Where(x => x.ValidationOutcome != "").Select(x => x.ValidationOutcome).ToList();
        }

        private void LoadJSONActions(string filePath)
        {
            var actionsList = Utils.GetAutomationsObjectList(filePath); //"Automations\\bb\\bb.json"

            foreach (FSAutomatorAction action in actionsList)
            {
                if (!action.IsAuxiliary)
                {
                    automator.ActionList.Add(action);
                }
                else
                {
                    automator.AuxiliaryActionList.Add(action);
                }
            }

            return;
        }



        public void AddActionAfterPosition(int position, string actionJSON)
        {
            var jsonObject = JObject.Parse(actionJSON);
            var actionName = jsonObject["Name"].ToString();
            var actionParameters = jsonObject["Parameters"].ToString();
            var uniqueID = Guid.NewGuid().ToString();
            var stopOnError = (bool)jsonObject["StopOnError"];

            Type actionType = Utils.GetType(String.Format("FSAutomator.Backend.Actions.{0}", actionName));
            Activator.CreateInstance(actionType);

            var actionObject = JsonConvert.DeserializeObject(actionParameters, actionType, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            FSAutomatorAction action = new FSAutomatorAction(actionName, uniqueID, "Pending", actionParameters, actionObject, false, stopOnError);

            automator.ActionList.Insert(position + 1, action);
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
            return selectedIndex + 1;
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
            return selectedIndex - 1;
        }


        public bool ExportAutomation(string filename, string destinationPath, AutomationFile l_SAutomationFilesList)
        {
            var exportStatus = new Exporters().ExportAutomation(filename, destinationPath, this.automator.ActionList, l_SAutomationFilesList);
            return exportStatus;
        }

        public void ReceiveSimConnectMessage()
        {
            Trace.WriteLine("Receive in BackEnd!!");
            m_SimConnect?.ReceiveMessage();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnect");

            if (m_SimConnect != null)
            {
                /// Dispose serves the same purpose as SimConnect_Close()
                m_SimConnect.Dispose();
                m_SimConnect = null;
            }
        }

        public bool Connect(IntPtr m_hWnd, int WM_USER_SIMCONNECT)
        {
            Trace.WriteLine("Connect BackEnd");


            try
            {
                /// The constructor is similar to SimConnect_Open in the native API
                m_SimConnect = new SimConnect("Simconnect - FSAutomator", m_hWnd, (uint)WM_USER_SIMCONNECT, null, 0);

                /// Listen to connect and quit msgs
                m_SimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(Simconnect_OnRecvOpen);
                m_SimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(Simconnect_OnRecvQuit);

                /// Listen to exceptions
                m_SimConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(Simconnect_OnRecvException);

                return true;

            }
            catch (COMException ex)
            {
                Trace.WriteLine("Connection to KH failed: " + ex.Message);
                return false;
                //MessageBox.Show(String.Format("Could not connect to MSFS: {0}", ex.Message), "Connection Error");
            }

        }

        private void Simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            this.automator.connection = this.m_SimConnect;
            this.automator.flightModel = new FlightModel(this.m_SimConnect);
        }

        private static void Simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("An exception occurred Simconnect_OnRecvException: {0}", data.dwException.ToString());
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;

            //note llançar excepció amb event
        }

        private static void Simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("Simulator has exited. Closing connection and exiting Simulator module");
        }

        public void ImportAutomationFromFilePath(string filepath)
        {
            new Importers().ImportAutomationFromFilePath(filepath);
        }
    }
}
