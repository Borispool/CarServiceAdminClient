using System;
using System.Globalization;
using System.Windows.Data;

namespace CarServiceAdminClient.Converters
{
    public class EqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString().Equals(parameter?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true)
                return parameter;

            return Binding.DoNothing;
        }
    }
}