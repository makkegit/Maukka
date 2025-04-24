using System.Globalization;

namespace Maukka.Utilities.Converters.UI
{
    public class IsGreaterThanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var numValue = double.TryParse(value?.ToString(), out var numResult) ? numResult : 0;
            var parameterValue = double.TryParse(parameter?.ToString(), out var paramResult) ? paramResult : 0;

            return numValue > parameterValue;
        } 

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}