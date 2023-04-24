using FSAutomator.Backend;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FSAutomator.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<FSAutomatorAction> l_ActionListUI;
        private FSAutomatorAction l_SelectedActionListUI;
        private int l_SelectedIndexActionListUI = -1;
        private string l_SelectedAutomationName = "";

        BackendMain backend = null;

        private List<AutomationFile> l_AutomationFilesList;
        private AutomationFile l_SelectedItemAutomationFilesList;
        private bool b_EditMode = false;

        private bool b_ConnectionStatus = false;

        private ICommand? b_ButtonLoadActions;
        private ICommand? b_ButtonExecute;
        private ICommand? b_ButtonRemove;
        private ICommand? b_ButtonAdd;
        private ICommand? b_ButtonUp;
        private ICommand? b_ButtonDown;
        private ICommand? b_ButtonImportAutomation;
        private ICommand? b_ButtonValidate;
        private ICommand? b_ButtonConnect;
        private ICommand? b_ButtonSaveAs;
        private ICommand? b_ButtonExport;

        #region Properties

        public ObservableCollection<FSAutomatorAction> ActionListUI
        {
            get
            {
                l_ActionListUI = backend.GetActionList();
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
                var selectedItem = SelectedItemAutomationFilesList;
                l_AutomationFilesList = value;
                RaisePropertyChanged("AutomationFilesList");
                SelectedItemAutomationFilesList = selectedItem;
            }
        }

        public AutomationFile SelectedItemAutomationFilesList
        {
            get { return l_SelectedItemAutomationFilesList; }

            set
            {
                l_SelectedItemAutomationFilesList = value;
                RaisePropertyChanged("SelectedItemAutomationFilesList");
            }
        }

        public string SelectedAutomationName
        {
            get { return l_SelectedAutomationName; }

            set
            {
                l_SelectedAutomationName = value;
                RaisePropertyChanged("SelectedAutomationName");
            }
        }

        public FSAutomatorAction SelectedActionListUI
        {
            get { return l_SelectedActionListUI; }

            set
            {
                l_SelectedActionListUI = value;
            }
        }

        public int SelectedIndexActionListUI
        {
            get { return l_SelectedIndexActionListUI; }

            set
            {
                l_SelectedIndexActionListUI = value;
                RaisePropertyChanged("SelectedIndexActionListUI");
            }
        }

        public bool EditMode
        {
            get { return b_EditMode; }

            set
            {
                b_EditMode = value;
                RaisePropertyChanged("EditMode");
                ValidateActions();
            }
        }

        public List<string> ValidationOutcomeCleaned
        {
            get
            {
                var validationIssues = backend.GetValidationIssuesList() ?? new List<string>();
                var schemaValidationIssues = backend.GetJSONValidationIssuesList() ?? new List<string>();
                return Enumerable.Concat(validationIssues, schemaValidationIssues).ToList();
            }
            set
            {
                RaisePropertyChanged("ValidationOutcomeCleaned");
            }

        }

        public bool ConnectionStatus
        {
            get
            {
                return b_ConnectionStatus;
            }
            set
            {
                b_ConnectionStatus = value;
                RaisePropertyChanged("ConnectionStatus");
            }

        }

        #endregion

        #region ICommand
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

        #endregion

        public MainWindowViewModel()
        {
            backend = new BackendMain();
            ActionListUI = backend.GetActionList();

            SubscribeToEvents();

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

        private void SubscribeToEvents()
        {
            backend.SubscribeToReportStatusEvent(ReportEventReceiver);
            backend.SubscribeToConnectionStatusChangeEvent(ConnectionStatusReceiver);
            //backEnd.status.ReportStatusEvent += ReportEventReceiver;
            //backEnd.status.ConnectionStatusChangeEvent += ConnectionStatusReceiver;
        }

        #region Event Subscribers and receivers

        public void ReportEventReceiver(object sender, InternalMessage msg)
        {
            MessageBox.Show(msg.Message, msg.Type.ToString());
        }

        private void ConnectionStatusReceiver(object? sender, bool e)
        {
            this.ConnectionStatus = backend.IsConnectedToSim();
        }

        #endregion

        private void SaveCurrentAutomation(object obj)
        {
            var result = backend.SaveAutomation(SelectedItemAutomationFilesList, SelectedAutomationName);
            RefreshAutomationFilesList();

            backend.status.ReportStatus(result);
        }

        private void ExportCurrentAutomationAs(object obj)
        {
            var filename = SelectedAutomationName;

            var result = backend.ExportAutomation(filename, SelectedItemAutomationFilesList);

            backend.status.ReportStatus(result);

            RefreshAutomationFilesList();
        }
        private void RefreshAutomationFilesList()
        {
            var selectedAutomation = SelectedItemAutomationFilesList;
            AutomationFilesList = Utils.GetAutomationFilesList();
            SelectedItemAutomationFilesList = selectedAutomation;
            backend.ClearAutomationList();
        }

        private void MoveActionUp(object obj)
        {
            var selectedIndex = ActionListUI.IndexOf(SelectedActionListUI);

            SelectedIndexActionListUI = backend.MoveActionUp(selectedIndex);
            SelectedIndexActionListUI = -1;

        }

        private void MoveActionDown(object obj)
        {
            var selectedIndex = ActionListUI.IndexOf(SelectedActionListUI);
            SelectedIndexActionListUI = backend.MoveActionDown(selectedIndex);
            SelectedIndexActionListUI = -1;
        }

        private void RemoveAction(object obj)
        {
            var currentSelectedIndex = SelectedIndexActionListUI;

            backend.RemoveAction(currentSelectedIndex);

            SelectedIndexActionListUI = -1;
        }

        private void AddAction(object obj)
        {
            try
            {
                AddActionWindow addAction = new AddActionWindow();
                addAction.ShowDialog();
                var newActionJSON = addAction.FinalJSON;

                if (String.IsNullOrEmpty(newActionJSON))
                {
                    return;
                }

                var selectedIndex = ActionListUI.IndexOf(SelectedActionListUI);

                backend.AddJSONActionAfterPosition(selectedIndex, newActionJSON, l_SelectedItemAutomationFilesList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("An error ocurred and the action will not be added: {0}", ex.Message));
            }

        }


        private async void ExecuteTask(object commandParameter)
        {
            if (backend is not null)
            {
                await Task.Run(() =>
                {
                    backend.Execute();
                });
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

        private void Connect(object commandParameter)
        {
            if (this.ConnectionStatus)
            {
                backend.Disconnect();
            }
            else
            {
                backend.Connect();
            }
        }

        public void LoadActions(object obj)
        {
            if (SelectedItemAutomationFilesList is null)
            {
                return;
            }

            backend.LoadActions(SelectedItemAutomationFilesList);
            SelectedAutomationName = l_SelectedItemAutomationFilesList.FileName;
            ValidateActions();
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

                backend.ImportAutomationFromFilePath(filepath);

                MessageBox.Show("Import finished. If the automation references any DLL files, remember to import them as well");
            }

            RefreshAutomationFilesList();
        }

        private void InitializeNewAutomation()
        {
            SelectedItemAutomationFilesList = new AutomationFile("");
        }

        private void ValidateActions(object commandParameter = null)
        {
            if (backend != null)
            {
                backend.ValidateActions();
                this.ValidationOutcomeCleaned = backend.GetValidationIssuesList();
            }
        }
    }

}
