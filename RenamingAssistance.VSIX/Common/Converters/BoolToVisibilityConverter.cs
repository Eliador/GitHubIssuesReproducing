using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;

namespace RenamingAssistance.VSIX.Common.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        private const string Invers = "inverse";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool)value == (parameter?.ToString() == Invers) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
            {
                return false;
            }

            var visibilityValue = (Visibility)value;
            if (parameter is bool && (bool)parameter)
            {
                return visibilityValue == Visibility.Visible ? false : true;
            }

            return visibilityValue == Visibility.Visible ? true : false;
        }
    }
}
