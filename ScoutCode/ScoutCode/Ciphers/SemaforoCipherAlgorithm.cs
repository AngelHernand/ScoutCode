using System.Text;

namespace ScoutCode.Ciphers;

// Cifrado Semáforo: cada letra del alfabeto (A-Z, sin Ñ) se representa
// con una posición de banderas. La salida de Encrypt es el formato intermedio
// "SEMAFORO:h,o,l,a" que la UI interpreta para mostrar imágenes.
// Decrypt recibe ese mismo formato y devuelve el texto plano.
public class SemaforoCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Semáforo";
    public string SupportedCharacters => "A-Z (solo letras del alfabeto inglés, sin Ñ)";

    // Prefijo que identifica la salida simbólica
    private const string SemaforoPrefix = "SEMAFORO:";

    // Alfabeto soportado (A-Z sin Ñ)
    private static readonly char[] Alphabet =
    {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
    };

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
                keys.Add(" ");
                continue;
            }

            if (!IsSupportedLetter(c))
                continue; // ignorar caracteres no soportados (Ñ, números, signos)

            var key = char.ToLowerInvariant(c).ToString();
            keys.Add(key);
        }

        if (keys.Count == 0)
            return string.Empty;

        return SemaforoPrefix + string.Join(",", keys);
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Debe comenzar con el prefijo SEMAFORO:
        if (!input.StartsWith(SemaforoPrefix, StringComparison.OrdinalIgnoreCase))
            return "Error: formato inválido. Se espera SEMAFORO:a,b,c,...";

        var payload = input[SemaforoPrefix.Length..];
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

            if (trimmed.Length == 1 && IsSupportedLetter(trimmed[0]))
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
