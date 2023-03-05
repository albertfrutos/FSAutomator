using System.Windows;

namespace FSAutomator.UI
{
    /// <summary>
    /// Interaction logic for AddAction.xaml
    /// </summary>
    public partial class AddActionWindow : Window
    {
        public string finalJSON { get; set; } = "";
        public AddActionWindow()
        {
            InitializeComponent();
        }

        private void dataGridActions_SelectionChanged()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            finalJSON = txtJSON.Text;
            this.Close();
        }
    }
}
