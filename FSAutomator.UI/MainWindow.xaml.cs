using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace FSAutomator
{
    // https://learn.microsoft.com/en-us/windows/communitytoolkit/controls/datagrid_guidance/rowdetails
    // https://stackoverflow.com/questions/3829137/i-need-the-expand-collapse-for-rowdetailstemplate
    // https://www.google.com/search?q=RowDetailsTemplate+collapse&oq=RowDetailsTemplate+collapse&aqs=chrome..69i57j0i22i30l3j69i60l2.1643j0j7&sourceid=chrome&ie=UTF-8

    interface IBaseSimConnectWrapper
    {
        int GetUserSimConnectWinEvent();
        void ReceiveSimConnectMessage();
        void SetWindowHandle(IntPtr _hWnd);
        void Disconnect();
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected HwndSource GetHWinSource()
        {
            return PresentationSource.FromVisual(this) as HwndSource;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            GetHWinSource().AddHook(WndProc);
            if (this.DataContext is IBaseSimConnectWrapper oBaseSimConnectWrapper)
            {
                oBaseSimConnectWrapper.SetWindowHandle(GetHWinSource().Handle);
            }
        }

        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr hWParam, IntPtr hLParam, ref bool bHandled)
        {
            //Trace.WriteLine("Received!! {0}", iMsg.ToString());
            if (this.DataContext is IBaseSimConnectWrapper oBaseSimConnectWrapper)
            {
                try
                {
                    if (iMsg == oBaseSimConnectWrapper.GetUserSimConnectWinEvent())
                    {
                        Trace.WriteLine("RECEIVED FROM FS!! {0}", iMsg.ToString());
                        oBaseSimConnectWrapper.ReceiveSimConnectMessage();
                    }
                }
                catch
                {
                    oBaseSimConnectWrapper.Disconnect();
                }
            }

            return IntPtr.Zero;
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolToOppositeBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class ValidationStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value)
            {
                case true:
                    return new BitmapImage(new Uri(@"icons\validationPassed.ico", UriKind.Relative)); 
                case false:
                    return new BitmapImage(new Uri(@"icons\validationFailed.ico", UriKind.Relative));
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if ((string)value == "icons\validationPassed.png")
                    return true;
                else
                    return false;
            }
            return "no";
        }
    }




}
