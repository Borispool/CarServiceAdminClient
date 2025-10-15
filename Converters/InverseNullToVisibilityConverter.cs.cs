using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CarServiceAdminClient.Converters
{
    // Цей конвертер робить навпаки: елемент ВИДИМИЙ, якщо даних немає (null), і ПРИХОВАНИЙ, якщо вони є.
    // Ідеально для нашого тексту-заглушки.
    public class InverseNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}