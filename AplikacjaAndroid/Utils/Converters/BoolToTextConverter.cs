using System.Globalization;

namespace AplikacjaAndroid
{
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string param && param.Contains(";"))
            {
                var parts = param.Split(';');
                return (bool)value ? parts[1] : parts[0];
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }
}
