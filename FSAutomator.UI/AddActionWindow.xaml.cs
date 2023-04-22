using System.Windows;

namespace FSAutomator.UI
{
    /// <summary>
    /// Interaction logic for AddAction.xaml
    /// </summary>
    public partial class AddActionWindow : Window
    {
        public string FinalJSON { get; set; } = "";
        public AddActionWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FinalJSON = txtJSON.Text;
            this.Close();
        }
    }
}
