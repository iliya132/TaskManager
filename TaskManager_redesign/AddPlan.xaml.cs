using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TaskManager_redesign
{
    /// <summary>
    /// Interaction logic for AddPlan.xaml
    /// </summary>
    public partial class AddPlan : Window
    {
        public AddPlan()
        {
            try { 
            InitializeComponent();
            }catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

    }
}
