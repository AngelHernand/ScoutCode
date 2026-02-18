namespace ScoutCode.Ciphers;

/// <summary>
/// Utilidades compartidas para todos los algoritmos de cifrado scout.
/// </summary>
public static class CipherUtils
{
    /// <summary>
    /// Alfabeto español completo incluyendo la Ñ.
    /// </summary>
    public static readonly char[] SpanishAlphabet =
    {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','Ñ','O','P','Q','R','S','T','U','V','W','X','Y','Z'
    };

    /// <summary>
    /// Preserva el case original: si originalChar era minúscula, retorna newChar en minúscula.
    /// </summary>
    public static char PreserveCase(char originalChar, char newChar)
    {
        if (char.IsLower(originalChar))
            return char.ToLowerInvariant(newChar);
        return char.ToUpperInvariant(newChar);
    }

    /// <summary>
    /// Determina si un carácter es una letra del alfabeto español (A-Z + Ñ, ambos cases).
    /// </summary>
    public static bool IsLetterSpanish(char c)
    {
        var upper = char.ToUpperInvariant(c);
        return upper == 'Ñ' || (upper >= 'A' && upper <= 'Z');
    }

    /// <summary>
    /// Obtiene el índice de una letra en el alfabeto español (0-26).
    /// Retorna -1 si no es una letra válida.
    /// </summary>
    public static int GetSpanishIndex(char c)
    {
        var upper = char.ToUpperInvariant(c);
        for (int i = 0; i < SpanishAlphabet.Length; i++)
        {
            if (SpanishAlphabet[i] == upper)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Obtiene la letra del alfabeto español en la posición dada (con wrap-around).
    /// </summary>
    public static char GetSpanishLetter(int index)
    {
        int len = SpanishAlphabet.Length; // 27
        int wrapped = ((index % len) + len) % len;
        return SpanishAlphabet[wrapped];
    }

    /// <summary>
    /// Aplica un mapa de sustitución char→char preservando case.
    /// Cualquier carácter no encontrado en el mapa se copia igual.
    /// El mapa debe usar MAYÚSCULAS como keys.
    /// </summary>
    public static string ApplyCharMap(string input, Dictionary<char, char> map)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new System.Text.StringBuilder(input.Length);
        foreach (char c in input)
        {
            var upper = char.ToUpperInvariant(c);
            if (map.TryGetValue(upper, out var replacement))
                sb.Append(PreserveCase(c, replacement));
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
}
