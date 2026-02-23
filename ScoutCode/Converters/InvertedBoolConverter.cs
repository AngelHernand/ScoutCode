using System.Globalization;

namespace ScoutCode.Converters;

/// <summary>
/// Invierte un booleano. Lo uso para habilitar/deshabilitar controles
/// y mostrar/ocultar elementos cuando la condición es negada.
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && !boolValue;
    }
}
