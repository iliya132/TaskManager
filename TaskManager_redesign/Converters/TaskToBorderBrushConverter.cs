using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

using TaskManager_redesign.Model;
using TaskManager_redesign.ViewModel;

namespace TaskManager_redesign.Converters
{
    public class TaskToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserTask task = value as UserTask;
            Analytic currentAnalytic = MainViewModel.GetInstance().CurrentAnalytic;
            if (task == null || currentAnalytic == null)
            {
                return new SolidColorBrush(Color.FromRgb(244,244,244));
            }
            if(task.AssignedTo.Any(i=>i.AnalyticId == currentAnalytic.Id))
            {
                return new SolidColorBrush(Colors.AliceBlue);
            }
            if(task.CreatedById == currentAnalytic.Id)
            {
                return new SolidColorBrush(Colors.DarkOrange);
            }
            return new SolidColorBrush(Color.FromRgb(244, 244, 244));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
