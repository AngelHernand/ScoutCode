using System.Text;
using System.Text.RegularExpressions;

namespace ScoutCode.Ciphers;

// Cifrado numerico: cada letra del alfabeto español se convierte en su posicion
// con 2 digitos. A=00, B=01 ... Ñ=14 ... Z=26.
public partial class NumericCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Numérica";
    public string SupportedCharacters => "A-Z, Ñ → 00-26 (resto se mantiene igual)";

    // Alfabeto español para esta clave
    private static readonly char[] Alphabet = CipherUtils.SpanishAlphabet;

    private static readonly Dictionary<char, string> LetterToNumber;
    private static readonly Dictionary<string, char> NumberToLetter;

    static NumericCipherAlgorithm()
    {
        LetterToNumber = new Dictionary<char, string>();
        NumberToLetter = new Dictionary<string, char>();
        for (int i = 0; i < Alphabet.Length; i++)
        {
            var num = i.ToString("D2"); // 00, 01, 02 ...
            LetterToNumber[Alphabet[i]] = num;
            NumberToLetter[num] = Alphabet[i];
        }
    }

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder();
        foreach (char c in input)
        {
            var upper = char.ToUpperInvariant(c);
            if (LetterToNumber.TryGetValue(upper, out var num))
                sb.Append(num);
            else
                sb.Append(c); // espacios, signos, etc. se copian
        }
        return sb.ToString();
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            // Intentar leer 2 dígitos consecutivos
            if (i + 1 < input.Length &&
                char.IsDigit(input[i]) && char.IsDigit(input[i + 1]))
            {
                var pair = input.Substring(i, 2);
                if (NumberToLetter.TryGetValue(pair, out var letter))
                {
                    sb.Append(letter);
                    i += 2;
                    continue;
                }
            }

            // no es un par valido, copio el caracter
            sb.Append(input[i]);
            i++;
        }

        return sb.ToString();
    }
}
