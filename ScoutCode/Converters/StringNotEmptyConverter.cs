using System.Globalization;

namespace ScoutCode.Converters;

// Devuelve true si el string no esta vacio. Lo uso para mostrar/ocultar cosas en el XAML.
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
