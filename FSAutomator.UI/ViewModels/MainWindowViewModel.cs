using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using Microsoft.FlightSimulator.SimConnect;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend;
using Microsoft.Win32;
using System.Reflection;
using FSAutomator.UI;
using FSAutomator.Backend.Utilities;
using FSAutomator.Backend.Actions;
using System.Diagnostics;
using System.IO.Compression;
using FSAutomator.BackEnd;
using FSAutomator.BackEnd.Entities;

namespace FSAutomator.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged, IBaseSimConnectWrapper
    {
        private ObservableCollection<FSAutomatorAction> l_ActionListUI;
        private FSAutomatorAction l_SActionListUI;
        private int l_SIActionListUI = -1;
        private string l_SAutomationName = "";

        BackendMain backEnd = null;

        public const int WM_USER_SIMCONNECT = 0x0402;
        private IntPtr m_hWnd = new IntPtr(0);

        private List<AutomationFile> l_AutomationFilesList;
        private AutomationFile l_SAutomationFilesList;
        private bool b_EditMode = false;




        private ICommand? b_ButtonLoadActions;
        private ICommand? b_ButtonExecute;
        private ICommand? b_ButtonRemove;
        private ICommand? b_ButtonAdd;
        private ICommand? b_ButtonUp;
        private ICommand? b_ButtonDown;
        private ICommand? b_ButtonImportAutomation;
        private ICommand? b_ButtonValidate;
        private ICommand? b_ButtonConnect;
        private ICommand b_ButtonSaveAs;
        private ICommand b_ButtonExport;

        public ICommand ButtonLoadActions
        {
            get
            {
                return b_ButtonLoadActions;
            }
            set
            {
                b_ButtonLoadActions = value;
            }
        }
        public ICommand ButtonExecute
        {
            get
            {
                return b_ButtonExecute;
            }
            set
            {
                b_ButtonExecute = value;
            }
        }

        public ICommand ButtonRemove
        {
            get
            {
                return b_ButtonRemove;
            }
            set
            {
                b_ButtonRemove = value;
            }
        }

        public ICommand ButtonAdd
        {
            get
            {
                return b_ButtonAdd;
            }
            set
            {
                b_ButtonAdd = value;
            }
        }

        public ICommand ButtonUp
        {
            get
            {
                return b_ButtonUp;
            }
            set
            {
                b_ButtonUp = value;
            }
        }

        public ICommand ButtonDown
        {
            get
            {
                return b_ButtonDown;
            }
            set
            {
                b_ButtonDown = value;
            }
        }

        public ICommand ButtonImportAutomation
        {
            get
            {
                return b_ButtonImportAutomation;
            }
            set
            {
                b_ButtonImportAutomation = value;
            }
        }

        public ICommand ButtonSaveAs
        {
            get
            {
                return b_ButtonSaveAs;
            }
            set
            {
                b_ButtonSaveAs = value;
            }
        }

        public ICommand ButtonExport
        {
            get
            {
                return b_ButtonExport;
            }
            set
            {
                b_ButtonExport = value;
            }
        }

        public ICommand ButtonValidate
        {
            get
            {
                return b_ButtonValidate;
            }
            set
            {
                b_ButtonValidate = value;
            }
        }

        public ICommand ButtonConnect
        {
            get
            {
                return b_ButtonConnect;
            }
            set
            {
                b_ButtonConnect = value;
            }
        }

        public void ErrorReceiver(object sender, string data)
        {
            MessageBox.Show(data);
            Trace.WriteLine("ui " + data);
        }

        public MainWindowViewModel()
        {
            backEnd = new BackendMain();//handle

            backEnd.status.Report += ErrorReceiver;

            ActionListUI = backEnd.GetActionList();


            RefreshAutomationFilesList();
            InitializeNewAutomation();


            ButtonLoadActions = new RelayCommand(new Action<object>(LoadActions));
            ButtonRemove = new RelayCommand(new Action<object>(RemoveAction));
            ButtonAdd = new RelayCommand(new Action<object>(AddAction));
            ButtonUp = new RelayCommand(new Action<object>(MoveActionUp));
            ButtonDown = new RelayCommand(new Action<object>(MoveActionDown));
            ButtonImportAutomation = new RelayCommand(ImportAutomation);
            ButtonValidate = new RelayCommand(new Action<object>(ValidateActions));
            ButtonExecute = new RelayCommand(new Action<object>(ExecuteTask));
            ButtonConnect = new RelayCommand(new Action<object>(Connect));
            ButtonSaveAs = new RelayCommand(new Action<object>(SaveCurrentAutomation));
            ButtonExport = new RelayCommand(new Action<object>(ExportCurrentAutomationAs));



        }

        private void test(object sender, string error)
        {
            MessageBox.Show(error);
        }

        private void SaveCurrentAutomation(object obj)
        {
            var filename = SAutomationName;

            if (!(filename.Length > 0))
            {
                MessageBox.Show("Please enter an automation name.", "Error");
                return;
            }

            var saveResult = backEnd.SaveAutomation(SAutomationFilesList, SAutomationName);
            RefreshAutomationFilesList();
            
            MessageBox.Show(saveResult);
        }



        private void ExportCurrentAutomationAs(object obj)
        {
            var filename = SAutomationName;
            var destinationPath = @"Exports";

            if (!(filename.Length > 0))
            {
                MessageBox.Show("Please enter an automation name.", "Error");
                return;
            }
            else if (backEnd.ExportAutomation(filename, destinationPath, SAutomationFilesList))
            {
                MessageBox.Show("Export finished.", "Finished");
            }
            else
            {
                MessageBox.Show("There is nothing to save.", "Error");
                return;
            }

            backEnd.ClearAutomationList();
            //this.ValidationOutcomeCleaned = backEnd.GetValidationIssuesList();

            RefreshAutomationFilesList();
        }

        private void RefreshAutomationFilesList()
        {
            var selectedAutomation = SAutomationFilesList;
            AutomationFilesList = Utils.GetAutomationFilesList();
            SAutomationFilesList = selectedAutomation;
        }


        private void MoveActionUp(object obj)
        {
            var selectedIndex = ActionListUI.IndexOf(SActionListUI);


            SIActionListUI = backEnd.MoveActionUp(selectedIndex);
            SIActionListUI = -1;

        }

        private void MoveActionDown(object obj)
        {
            var selectedIndex = ActionListUI.IndexOf(SActionListUI);
            SIActionListUI = backEnd.MoveActionDown(selectedIndex);
            SIActionListUI = -1;

        }



        private void RemoveAction(object obj)
        {
            var currentSelectedIndex = SIActionListUI;

            backEnd.RemoveAction(currentSelectedIndex);

            SIActionListUI = -1;
        }

        private void AddAction(object obj)
        {
            try
            {
                AddActionWindow addAction = new AddActionWindow();
                addAction.DataContext = new AddActionViewModel();
                addAction.ShowDialog();
                var newActionJSON = addAction.finalJSON;

                if (String.IsNullOrEmpty(newActionJSON))
                {
                    return;
                }

                var selectedIndex = ActionListUI.IndexOf(SActionListUI);

                backEnd.AddJSONActionAfterPosition(selectedIndex, newActionJSON, l_SAutomationFilesList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("An error ocurred and the action will not be added: {0}", ex.Message));
            }

        }


        private async void ExecuteTask(object commandParameter)
        {

            if (backEnd is not null)
            {
                await Task.Run(() =>
                {
                    backEnd.Execute();
                });
            }

        }



        public void SetWindowHandle(IntPtr _hWnd)
        {
            m_hWnd = _hWnd;
        }

        public ObservableCollection<FSAutomatorAction> ActionListUI
        {
            get {
                l_ActionListUI = backEnd.GetActionList();
                return l_ActionListUI;
            }

            set
            {
                l_ActionListUI = value;
                RaisePropertyChanged("ActionListUI");
            }
        }

        public List<AutomationFile> AutomationFilesList
        {
            get { return l_AutomationFilesList; }

            set
            {
                var selectedItem = SAutomationFilesList;
                l_AutomationFilesList = value;
                RaisePropertyChanged("AutomationFilesList");
                SAutomationFilesList = selectedItem;

            }
        }

        public AutomationFile SAutomationFilesList
        {
            get { return l_SAutomationFilesList; }

            set
            {
                l_SAutomationFilesList = value;
                RaisePropertyChanged("SAutomationFilesList");
            }
        }

        public string SAutomationName
        {
            get { return l_SAutomationName; }

            set
            {
                l_SAutomationName = value;
                RaisePropertyChanged("SAutomationName");
            }
        }

        public FSAutomatorAction SActionListUI
        {
            get { return l_SActionListUI; }

            set
            {
                l_SActionListUI = value;
            }
        }

        public int SIActionListUI
        {
            get { return l_SIActionListUI; }

            set
            {
                l_SIActionListUI = value;
                RaisePropertyChanged("SIActionListUI");
            }
        }

        public bool EditMode
        {
            get { return b_EditMode; }

            set
            {
                b_EditMode = value;
                RaisePropertyChanged("EditMode");
            }
        }

        public List<string> ValidationOutcomeCleaned
        {
            get
            {
                return backEnd.status.ValidationIssues; //l_ValidationOutcomeCleaned;

            }
            set
            {
                backEnd.status.ValidationIssues = value;
                RaisePropertyChanged("ValidationOutcomeCleaned");
            }

        }

        public bool ConnectionStatus
        {
            get
            {
                return backEnd.status.IsConnectedToSim; //l_ValidationOutcomeCleaned;

            }
            set
            {
                backEnd.status.IsConnectedToSim = value;
                RaisePropertyChanged("ConnectionStatus");
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                Task.Run(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)));
            }
        }


        public void ReceiveSimConnectMessage()
        {
            Trace.WriteLine("Receive from Model!!");
            backEnd.ReceiveSimConnectMessage();
        }


        public int GetUserSimConnectWinEvent()
        {
            return WM_USER_SIMCONNECT;
        }



        public void Disconnect()
        {
            Trace.WriteLine("Disconnect ViewModel");
            backEnd.Disconnect();
        }


        private void Connect(object commandParameter)
        {
            Trace.WriteLine("Connect ViewModel");
            backEnd.Connect(m_hWnd, WM_USER_SIMCONNECT);


        }

        public void LoadActions(object obj)
        {

            if (SAutomationFilesList is null)
            {
                return;
            }

            //SAutomationName = Path.GetFileNameWithoutExtension(SAutomationFilesList.FileName);

            backEnd.LoadActions(SAutomationFilesList);
            SAutomationName = l_SAutomationFilesList.FileName;
            this.ValidationOutcomeCleaned = backEnd.GetValidationIssuesList();
        }


        private void ImportAutomation(object commandParameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ofd.Filter = "AutomationFiles (*.json; *.dll; *.zip) |*.json;*.dll;*.zip| JSON |*.json| DLL |*.dll| Automation Package ZIP (*.zip) |*.zip";

            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == true)
            {
                var filepath = ofd.FileName;

                backEnd.ImportAutomationFromFilePath(filepath);

                MessageBox.Show("Import finished. If the automation references any DLL files, remember to import them as well");
            }

            RefreshAutomationFilesList();
        }


        private void InitializeNewAutomation()
        {
            SAutomationFilesList = new AutomationFile("");
        }





        private void ValidateActions(object commandParameter)
        {
            if (backEnd != null)
            {
                this.ValidationOutcomeCleaned = backEnd.ValidateActions();
            }
        }
    }

}
