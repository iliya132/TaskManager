using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
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

        Point startPoint;
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                TreeViewItem listViewItem = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                UserTask task = (UserTask)listViewItem.Header;

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", task);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T t)
                {
                    return t;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void Bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(sender as TreeView);
            }
        }

        private bool IsTaskToTaskActionExecuted;
        private void Bd_Drop(object sender, DragEventArgs e)
        {
            if (IsTaskToTaskActionExecuted)
            {
                IsTaskToTaskActionExecuted = false;
                return;
            }
            if (e.Data.GetDataPresent("myFormat"))
            {
                UserTask task = e.Data.GetData("myFormat") as UserTask;
                TreeViewItem treeViewItm = FindAnchestor<TreeViewItem>((DependencyObject)sender);
                UserTask NewParentTask = null;
                if (treeViewItm != null)
                {
                    NewParentTask = treeViewItm.Header as UserTask;
                    IsTaskToTaskActionExecuted = true;
                }
                MainViewModel mvm = MainViewModel.GetInstance();
                mvm.DragNDrop.Execute((task, NewParentTask));
            }
        }

        private void Bd_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TaskTree_Loaded(object sender, RoutedEventArgs e)
        {
            if(TaskTree.Items.Count > 0)
            {
                if (TaskTree.ItemContainerGenerator.ContainerFromItem(TaskTree.Items[0]) is TreeViewItem tvi)
                {
                    tvi.IsSelected = true;
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                TaskTree.Focus();
            }
        }

        private void AddNewTaskField_KeyDown(object sender, KeyEventArgs e)
        {

            if(e.Key == Key.Enter)
            {
                TaskTree.Focus();
            }else if(e.Key == Key.Escape)
            {
                (sender as TextBox).Text = string.Empty;
                TaskTree.Focus();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            TextInputWindow textInput = new TextInputWindow();
            textInput.Owner = this;
            textInput.Title = "Введите комментарий";
            textInput.MessageText.Text = "Пожалуйста укажите комментарий к проделанной работе";
            textInput.ShowDialog();
        }

        private void AddAssignedAnalyticBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectEmployee selectEmployeeView = new SelectEmployee();
            selectEmployeeView.Owner = this;
            selectEmployeeView.FilterTextBox.Focus();
            selectEmployeeView.ShowDialog();
        }

        private void AddPlanStepBtn_Click(object sender, RoutedEventArgs e)
        {
            AddPlan window = new AddPlan();
            window.Owner = this;
            window.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Activated(object sender, EventArgs e)
        {
            UpdateService.CheckForUpdate();
        }

        //private void TaskTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    if (e.NewValue == null)
        //        return;
        //    UserTask oldTask = e.OldValue as UserTask;
        //    UserTask ut = e.NewValue as UserTask;
        //    if (ut.IsHeader)
        //    {
        //        ut.IsSelected = false;
        //    }



        //}
    }
}
