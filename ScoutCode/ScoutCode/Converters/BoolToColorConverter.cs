using System.Globalization;

namespace ScoutCode.Converters;

/// <summary>
/// Converts a boolean to one of two colors. TrueColor / FalseColor via parameters or defaults.
/// Usage in XAML: pass "TrueHex|FalseHex" as ConverterParameter, e.g. "#4a7a4e|#34657f"
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Color.FromArgb("#4a7a4e");
    public Color FalseColor { get; set; } = Color.FromArgb("#34657f");

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool boolValue = value is true;

        if (parameter is string paramStr && paramStr.Contains('|'))
        {
            var parts = paramStr.Split('|');
            if (parts.Length == 2)
            {
                return boolValue
                    ? Color.FromArgb(parts[0].Trim())
                    : Color.FromArgb(parts[1].Trim());
            }
        }

        return boolValue ? TrueColor : FalseColor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
