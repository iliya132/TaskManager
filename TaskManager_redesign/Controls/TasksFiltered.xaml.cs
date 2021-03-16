using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManager_redesign.Controls
{
    /// <summary>
    /// Interaction logic for TasksFiltered.xaml
    /// </summary>
    public partial class TasksFiltered : UserControl
    {
        public TasksFiltered()
        {
            InitializeComponent();
        }

        private void AddNewTaskField_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                TasksGrid.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = string.Empty;
                TasksGrid.Focus();
            }
        }
    }
}
