using System.Text;

namespace ScoutCode.Ciphers;

// Cifrado Gato (Pigpen): cada letra del alfabeto español se representa
// con un símbolo gráfico. La salida de Encrypt es el formato intermedio
// "GATO:h,o,l,a" que la UI interpreta para mostrar imágenes.
// Decrypt recibe ese mismo formato y devuelve el texto plano.
public class GatoCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Gato (Pigpen)";
    public string SupportedCharacters => "A-Z, Ñ (solo letras del alfabeto español)";

    // Prefijo que identifica la salida simbólica
    private const string GatoPrefix = "GATO:";

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var keys = new List<string>();

        foreach (char c in input)
        {
            if (c == ' ')
            {
                keys.Add(" ");
                continue;
            }

            if (!CipherUtils.IsLetterSpanish(c))
                continue; // ignorar caracteres no soportados

            var upper = char.ToUpperInvariant(c);
            // Ñ → clave especial "enie"
            var key = upper == 'Ñ' ? "enie" : upper.ToString().ToLowerInvariant();
            keys.Add(key);
        }

        if (keys.Count == 0)
            return string.Empty;

        return GatoPrefix + string.Join(",", keys);
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Debe comenzar con el prefijo GATO:
        if (!input.StartsWith(GatoPrefix, StringComparison.OrdinalIgnoreCase))
            return "Error: formato inválido. Se espera GATO:a,b,c,...";

        var payload = input[GatoPrefix.Length..];
        if (string.IsNullOrWhiteSpace(payload))
            return string.Empty;

        var keys = payload.Split(',');
        var sb = new StringBuilder();

        foreach (var key in keys)
        {
            var trimmed = key.Trim();
            if (trimmed == " " || trimmed == "")
            {
                sb.Append(' ');
                continue;
            }

            if (trimmed.Equals("enie", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append('Ñ');
                continue;
            }

            if (trimmed.Length == 1 && CipherUtils.IsLetterSpanish(trimmed[0]))
            {
                sb.Append(char.ToUpperInvariant(trimmed[0]));
            }
            else
            {
                sb.Append('?'); // clave no reconocida
            }
        }

        return sb.ToString();
    }
}
