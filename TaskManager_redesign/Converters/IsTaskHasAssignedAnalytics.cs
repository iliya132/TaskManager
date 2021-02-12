using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TaskManager_redesign.Model;

namespace TaskManager_redesign.Converters
{
    public class IsTaskHasAssignedAnalytics : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is UserTask task))
            {
                return Visibility.Visible;
            }
            return task.AssignedTo.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
