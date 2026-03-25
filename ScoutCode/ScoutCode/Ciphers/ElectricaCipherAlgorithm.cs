using System.Text;

namespace ScoutCode.Ciphers;

// Cifrado Eléctrica: cada letra (A-Z, sin Ñ) se representa con un símbolo
// basado en posiciones de líneas eléctricas. La salida de Encrypt es
// "ELECTRICA:h,o,l,a" y la UI lo traduce a imágenes.
// Los espacios se representan con la clave "space".
public class ElectricaCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Eléctrica";
    public string SupportedCharacters => "A-Z (solo letras del alfabeto inglés, sin Ñ). Soporta espacios.";

    private const string Prefix = "ELECTRICA:";

    private static bool IsSupportedLetter(char c)
    {
        var upper = char.ToUpperInvariant(c);
        return upper >= 'A' && upper <= 'Z';
    }

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var keys = new List<string>();

        foreach (char c in input)
        {
            if (c == ' ')
            {
                keys.Add("space");
                continue;
            }

            if (!IsSupportedLetter(c))
                continue; // ignorar Ñ, números, signos

            var key = char.ToLowerInvariant(c).ToString();
            keys.Add(key);
        }

        if (keys.Count == 0)
            return string.Empty;

        return Prefix + string.Join(",", keys);
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        if (!input.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            return "Error: formato inválido. Se espera ELECTRICA:a,b,c,...";

        var payload = input[Prefix.Length..];
        if (string.IsNullOrWhiteSpace(payload))
            return string.Empty;

        var keys = payload.Split(',');
        var sb = new StringBuilder();

        foreach (var key in keys)
        {
            var trimmed = key.Trim();

            if (trimmed.Equals("space", StringComparison.OrdinalIgnoreCase)
                || trimmed == " " || trimmed == "")
            {
                sb.Append(' ');
                continue;
            }

            if (trimmed.Length == 1 && IsSupportedLetter(trimmed[0]))
            {
                sb.Append(char.ToUpperInvariant(trimmed[0]));
            }
            else
            {
                sb.Append('?');
            }
        }

        return sb.ToString();
    }
}
