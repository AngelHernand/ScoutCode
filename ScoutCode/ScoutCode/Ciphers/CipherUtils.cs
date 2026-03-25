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

    // Normaliza texto OCR para Morse: corrige caracteres que el OCR confunde
    // con puntos, rayas y separadores de palabra.
    public static string NormalizeMorseOcr(string ocrText)
    {
        if (string.IsNullOrEmpty(ocrText))
            return string.Empty;

        var sb = new System.Text.StringBuilder(ocrText.Length);

        foreach (char c in ocrText)
        {
            switch (c)
            {
                // Puntos: el OCR puede leer ·, •, ∙, ●, ◦, *, °, o incluso ,
                case '.':
                case ',':
                case '\u00B7': // ·
                case '\u2022': // •
                case '\u2219': // ∙
                case '\u25CF': // ●
                case '\u25E6': // ◦
                case '*':
                case '\u00B0': // °
                    sb.Append('.');
                    break;

                // Rayas: el OCR puede leer —, –, _, ~, ‐, ‑, ―
                case '-':
                case '\u2014': // —
                case '\u2013': // –
                case '\u2012': // ‐
                case '\u2011': // ‑
                case '\u2015': // ―
                case '_':
                case '~':
                    sb.Append('-');
                    break;

                // Separador de palabras: |, \, l (ele minúscula aislada se deja)
                case '/':
                case '|':
                case '\\':
                    sb.Append('/');
                    break;

                // Espacios y saltos de línea → espacio simple
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    sb.Append(' ');
                    break;

                // Caracteres Morse válidos que no necesitan conversión
                default:
                    // Ignorar caracteres basura del OCR que no son Morse
                    if (char.IsLetterOrDigit(c))
                        sb.Append(c);
                    break;
            }
        }

        // Limpiar espacios múltiples y normalizar " / "
        var result = sb.ToString();
        // Asegurar formato " / " para separación de palabras
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\s*/\s*", " / ");
        // Colapsar espacios múltiples (pero preservar " / ")
        result = System.Text.RegularExpressions.Regex.Replace(result, @"(?<! /) {2,}(?!/ )", " ");
        return result.Trim();
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
