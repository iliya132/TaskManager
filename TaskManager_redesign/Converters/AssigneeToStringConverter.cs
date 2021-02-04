using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

using TaskManager_redesign.Model;

namespace TaskManager_redesign.Converters
{
    public class AssigneeToStringConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            List<Analytic> analytics = value as List<Analytic>;
            return string.Join(", ", analytics.Select(a=>$"{a.LastName} {a.FirstName[0]}.{a.FatherName[0]}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
