using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using TaskManager_redesign.Model;

namespace TaskManager_redesign.Converters
{
    public class TaskPlanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if(value is TaskPlan plan)
            {
                return $"{plan.DueDate:dd.MM.yyyy} - {plan.Description}";
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
