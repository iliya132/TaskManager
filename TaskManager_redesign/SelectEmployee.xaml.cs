using System.Windows;

namespace TaskManager_redesign
{
    /// <summary>
    /// Interaction logic for SelectEmployee.xaml
    /// </summary>
    public partial class SelectEmployee : Window
    {
        public SelectEmployee()
        {
            InitializeComponent();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FilterTextBox.Clear();
        }
    }
}
