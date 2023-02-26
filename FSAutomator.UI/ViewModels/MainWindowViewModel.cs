using FSAutomator.Backend;
using FSAutomator.Backend.Entities;
using FSAutomator.Backend.Utilities;
using FSAutomator.BackEnd.Entities;
using FSAutomator.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        BackendMain backEnd = null;

        private List<AutomationFile> l_AutomationFilesList;
        private AutomationFile l_SelectedItemAutomationFilesList;
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
        private ICommand? b_ButtonSaveAs;
        private ICommand? b_ButtonExport;

        #region Properties

        public ObservableCollection<FSAutomatorAction> ActionListUI
        {
            get
            {
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
            }
        }

        public List<string> ValidationOutcomeCleaned
        {
            get
            {
                return backEnd.GetValidationIssuesList(); 
            }
            set {
                RaisePropertyChanged("ValidationOutcomeCleaned");
            }

        }

        public bool ConnectionStatus
        {
            get
            {
                return backEnd.status.IsConnectedToSim;
            }
            set
            {
                backEnd.status.IsConnectedToSim = value;
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
            backEnd = new BackendMain();
            ActionListUI = backEnd.GetActionList();

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
            backEnd.status.ReportErrorEvent += ReportEventReceiver;
            backEnd.status.ConnectionStatusChangeEvent += ConnectionStatusReceiver;
        }

        #region Event Receivers

        public void ReportEventReceiver(object sender, InternalMessage msg)
        {
            MessageBox.Show(msg.Message);
        }

        private void ConnectionStatusReceiver(object? sender, bool e)
        {
            this.ConnectionStatus = backEnd.status.IsConnectedToSim;
        }

        #endregion

        private void SaveCurrentAutomation(object obj)
        {
            var result = backEnd.SaveAutomation(SelectedItemAutomationFilesList, SelectedAutomationName);
            RefreshAutomationFilesList();

            backEnd.status.ReportError(result);
        }

        private void ExportCurrentAutomationAs(object obj)
        {
            var filename = SelectedAutomationName;
            var destinationPath = @"Exports";

            var result = backEnd.ExportAutomation(filename, destinationPath, SelectedItemAutomationFilesList);

            RefreshAutomationFilesList();
        }
        private void RefreshAutomationFilesList()
        {
            var selectedAutomation = SelectedItemAutomationFilesList;
            AutomationFilesList = Utils.GetAutomationFilesList();
            SelectedItemAutomationFilesList = selectedAutomation;
        }

        private void MoveActionUp(object obj)
        {
            var selectedIndex = ActionListUI.IndexOf(SelectedActionListUI);

            SelectedIndexActionListUI = backEnd.MoveActionUp(selectedIndex);
            SelectedIndexActionListUI = -1;

        }

        private void MoveActionDown(object obj)
        {
            var selectedIndex = ActionListUI.IndexOf(SelectedActionListUI);
            SelectedIndexActionListUI = backEnd.MoveActionDown(selectedIndex);
            SelectedIndexActionListUI = -1;
        }

        private void RemoveAction(object obj)
        {
            var currentSelectedIndex = SelectedIndexActionListUI;

            backEnd.RemoveAction(currentSelectedIndex);

            SelectedIndexActionListUI = -1;
        }

        private void AddAction(object obj)
        {
            try
            {
                AddActionWindow addAction = new AddActionWindow();
                addAction.ShowDialog();
                var newActionJSON = addAction.finalJSON;

                if (String.IsNullOrEmpty(newActionJSON))
                {
                    return;
                }

                var selectedIndex = ActionListUI.IndexOf(SelectedActionListUI);

                backEnd.AddJSONActionAfterPosition(selectedIndex, newActionJSON, l_SelectedItemAutomationFilesList);
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
                backEnd.Disconnect();
            }
            else
            {
                backEnd.Connect();
            }
        }

        public void LoadActions(object obj)
        {

            if (SelectedItemAutomationFilesList is null)
            {
                return;
            }

            backEnd.LoadActions(SelectedItemAutomationFilesList);
            SelectedAutomationName = l_SelectedItemAutomationFilesList.FileName;
            ValidateActions(null);
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
            SelectedItemAutomationFilesList = new AutomationFile("");
        }





        private void ValidateActions(object commandParameter)
        {
            if (backEnd != null)
            {
                backEnd.ValidateActions();
                this.ValidationOutcomeCleaned = backEnd.GetValidationIssuesList();
            }
        }
    }

}
