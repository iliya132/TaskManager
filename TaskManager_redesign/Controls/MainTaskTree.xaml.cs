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
using TaskManager_redesign.Model;
using TaskManager_redesign.ViewModel;

namespace TaskManager_redesign.Controls
{
    /// <summary>
    /// Interaction logic for MainTaskTree.xaml
    /// </summary>
    public partial class MainTaskTree : UserControl
    {
        public MainTaskTree()
        {
            InitializeComponent();
        }
        private void Grid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDownHitted = false;
        }
        Point startPoint;
        bool IsDragging = false;
        bool IsMouseDownHitted = false;
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                !IsDragging && IsMouseDownHitted)
            {
                Point mousePos = e.GetPosition(null);
                Vector diff = startPoint - mousePos;
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {

                    StartDrag(e);
                }
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            IsDragging = true;
            // Get the dragged ListViewItem
            TreeViewItem listViewItem = FindAnchestor<TreeViewItem>((DependencyObject)e.OriginalSource);

            // Find the data behind the ListViewItem
            UserTask task = (UserTask)listViewItem.Header;

            // Initialize the drag & drop operation
            DataObject dragData = new DataObject("myFormat", task);
            DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            IsDragging = false;
            e.Handled = true;
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
            IsMouseDownHitted = true;
            startPoint = e.GetPosition(null);
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
            e.Handled = true;
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
            if (TaskTree.Items.Count > 0)
            {
                if (TaskTree.ItemContainerGenerator.ContainerFromItem(TaskTree.Items[0]) is TreeViewItem tvi)
                {
                    tvi.IsSelected = true;
                }
            }
        }



        private void AddNewTaskField_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                TaskTree.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                (sender as TextBox).Text = string.Empty;
                TaskTree.Focus();
            }
        }

 
    }
}
