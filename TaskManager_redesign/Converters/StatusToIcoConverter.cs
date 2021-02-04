using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using TaskManager_redesign.Model;

namespace TaskManager_redesign.Converters
{
    public class StatusToIcoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            string status = (value as Status).Name;
            if(status.Equals("В работе"))
            {
                return new BitmapImage(new Uri(@"/res/AtWork.png", UriKind.Relative));
            }
            else
            {
                return new BitmapImage(new Uri(@"/res/Done.png", UriKind.Relative));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
