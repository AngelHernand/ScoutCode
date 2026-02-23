using System.Text;
using System.Text.RegularExpressions;

namespace ScoutCode.Ciphers;

// Cifrado tipo teclado de telefono viejo.
// Cada letra se mapea al numero del boton y cuantas veces se presiona.
// Ej: A=2, B=2^2, C=2^3. El espacio es 0. Se separan con guion.
public partial class CellphoneCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Celular (Teléfono)";
    public string SupportedCharacters => "A-Z (teclado T9), espacio=0, separador: -";

    private static readonly Dictionary<char, string> LetterToCode = new()
    {
        { 'A', "2" },   { 'B', "2^2" }, { 'C', "2^3" },
        { 'D', "3" },   { 'E', "3^2" }, { 'F', "3^3" },
        { 'G', "4" },   { 'H', "4^2" }, { 'I', "4^3" },
        { 'J', "5" },   { 'K', "5^2" }, { 'L', "5^3" },
        { 'M', "6" },   { 'N', "6^2" }, { 'O', "6^3" },
        { 'P', "7" },   { 'Q', "7^2" }, { 'R', "7^3" }, { 'S', "7^4" },
        { 'T', "8" },   { 'U', "8^2" }, { 'V', "8^3" },
        { 'W', "9" },   { 'X', "9^2" }, { 'Y', "9^3" }, { 'Z', "9^4" },
    };

    private static readonly Dictionary<string, char> CodeToLetter;

    static CellphoneCipherAlgorithm()
    {
        CodeToLetter = new Dictionary<string, char>();
        foreach (var kvp in LetterToCode)
            CodeToLetter[kvp.Value] = kvp.Key;
    }

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder();
        bool first = true;

        foreach (char c in input)
        {
            if (c == ' ')
            {
                if (!first) sb.Append('-');
                sb.Append('0');
                first = false;
                continue;
            }

            var upper = char.ToUpperInvariant(c);
            if (LetterToCode.TryGetValue(upper, out var code))
            {
                if (!first) sb.Append('-');
                sb.Append(code);
                first = false;
            }
            else
            {
                // no lo reconozco, lo dejo como esta
                if (!first) sb.Append('-');
                sb.Append(c);
                first = false;
            }
        }

        return sb.ToString();
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder();
        var tokens = input.Split('-');

        foreach (var token in tokens)
        {
            var trimmed = token.Trim();
            if (trimmed == "0")
            {
                sb.Append(' ');
            }
            else if (CodeToLetter.TryGetValue(trimmed, out var letter))
            {
                sb.Append(letter);
            }
            else
            {
                // no lo reconozco, lo dejo
                sb.Append(trimmed);
            }
        }

        return sb.ToString();
    }
}
