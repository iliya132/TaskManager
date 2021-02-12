using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace TaskManager_redesign.Converters
{
    class DateTimeToWindowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return $"{(DateTime)value:d}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Regex regex = new Regex(@"(\d{2})\.(\d{2})\.(\d{4})");

            string dateStr = regex.Match(value as string).Value;
            if (string.IsNullOrWhiteSpace(dateStr))
            {
                return null;
            }
            else
            {
                DateTime date;
                try
                {
                    date = System.Convert.ToDateTime(dateStr);
                }
                catch
                {
                    date = DateTime.Now;
                }
                return date;
            }
        }
    }
}
