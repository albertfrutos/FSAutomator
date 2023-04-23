using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FSAutomator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
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
