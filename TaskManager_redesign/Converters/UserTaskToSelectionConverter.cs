using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

using TaskManager_redesign.Model;

namespace TaskManager_redesign.Converters
{
    public class UserTaskToSelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values==null || values.Length == 0)
            {
                return false;
            }
            UserTask ViewModelTask = values[0] as UserTask;
            UserTask TreeViewTask = values[1] as UserTask;
            if(ViewModelTask!=null && TreeViewTask != null)
            {
                return ViewModelTask == TreeViewTask;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[2];
        }
    }
}
