using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager_redesign.Controls;
using TaskManager_redesign.Model;
using TaskManager_redesign.Services;
using TaskManager_redesign.ViewModel;

namespace TaskManager_redesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsExportSubMenuOpened { get; set; } = false;
        private bool IsStructureSubMenuOpened { get; set; } = false;
        const string DROP_DOWN_CLOSED = @"res\dropdown2.png";
        const string DROP_DOWN_OPENED = @"res\dropdown.png";
        MainTaskTree taskTree = new MainTaskTree();
        TasksFiltered tasksFiltered = new TasksFiltered();
        public MainWindow()
        {
            InitializeComponent();
            ContentHolder.Content = taskTree;
            TasksFilteredContent.Content = tasksFiltered;
            tasksFiltered.tasksList.MouseDoubleClick += TasksList_MouseDoubleClick;

        }

        private void TasksList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AllTasksBtn.Command.Execute("AllTasks");
            TasksFilteredContent.Visibility = Visibility.Collapsed;
            ContentHolder.Visibility = Visibility.Visible;
        }

        private void OpenCloseExportSubMenu(object sender, RoutedEventArgs e)
        {
            if (IsExportSubMenuOpened)
            {
                dropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                ExportSubMenu.Visibility = Visibility.Collapsed;
                IsExportSubMenuOpened = false;
            }
            else
            {
                
                dropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));

                ExportSubMenu.Visibility = Visibility.Visible;
                IsExportSubMenuOpened = true;
            }
        }

        private void OpenCloseStructureSubMenu(object sender, RoutedEventArgs e)
        {
            if (IsStructureSubMenuOpened)
            {
                StructureDropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_CLOSED, UriKind.Relative));
                StructureSubMenu.Visibility = Visibility.Collapsed;
                IsStructureSubMenuOpened = false;
            }
            else
            {

                StructureDropDownImg.Source = new BitmapImage(new Uri(DROP_DOWN_OPENED, UriKind.Relative));

                StructureSubMenu.Visibility = Visibility.Visible;
                IsStructureSubMenuOpened = true;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ContentHolder.Focus();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            TextInputWindow textInput = new TextInputWindow();
            textInput.Owner = Application.Current.MainWindow;
            textInput.Title = "Введите комментарий";
            textInput.MessageText.Text = "Пожалуйста укажите комментарий к проделанной работе";
            textInput.ShowDialog();
        }

        private void AddAssignedAnalyticBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectEmployee selectEmployeeView = new SelectEmployee();
            selectEmployeeView.Owner = Application.Current.MainWindow;
            selectEmployeeView.FilterTextBox.Focus();
            selectEmployeeView.ShowDialog();
        }

        private void AddPlanStepBtn_Click(object sender, RoutedEventArgs e)
        {
            AddPlan window = new AddPlan();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Activated(object sender, EventArgs e)
        {
            UpdateService.CheckForUpdate();
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            TasksFilteredContent.Visibility = Visibility.Visible;
            ContentHolder.Visibility = Visibility.Collapsed;
            
        }

        private void AllTasksBtnClick(object sender, RoutedEventArgs e)
        {
            TasksFilteredContent.Visibility = Visibility.Collapsed;
            ContentHolder.Visibility = Visibility.Visible;
            
        }
    }
}
