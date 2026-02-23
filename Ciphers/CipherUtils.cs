namespace ScoutCode.Ciphers;

// Utilidades que comparten todos los cifrados
public static class CipherUtils
{
    // Alfabeto español con la Ñ (27 letras)
    public static readonly char[] SpanishAlphabet =
    {
        'A','B','C','D','E','F','G','H','I','J','K','L','M',
        'N','Ñ','O','P','Q','R','S','T','U','V','W','X','Y','Z'
    };

    // Mantiene la mayuscula/minuscula del caracter original
    public static char PreserveCase(char originalChar, char newChar)
    {
        if (char.IsLower(originalChar))
            return char.ToLowerInvariant(newChar);
        return char.ToUpperInvariant(newChar);
    }

    public static bool IsLetterSpanish(char c)
    {
        var upper = char.ToUpperInvariant(c);
        return upper == 'Ñ' || (upper >= 'A' && upper <= 'Z');
    }

    // Devuelve la posicion de la letra en el alfabeto (0-26), o -1 si no es letra
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

    // Devuelve la letra en la posicion dada (hace wrap si se pasa del final)
    public static char GetSpanishLetter(int index)
    {
        int len = SpanishAlphabet.Length; // 27
        int wrapped = ((index % len) + len) % len;
        return SpanishAlphabet[wrapped];
    }

    // Aplica un mapa de sustitucion letra por letra, manteniendo mayusculas/minusculas.
    // Las keys del mapa tienen que estar en mayuscula.
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
