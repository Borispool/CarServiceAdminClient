using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CarServiceAdminClient.Converters
{
    // Цей конвертер робить елемент ВИДИМИМ, якщо дані є, і ПРИХОВАНИМ, якщо їх немає (null).
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}