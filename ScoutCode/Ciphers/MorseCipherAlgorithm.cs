using System.Text;

namespace ScoutCode.Ciphers;

// Cifrado Morse: letras y numeros a puntos y rayas.
// Las letras se separan con espacio, las palabras con " / ".
public class MorseCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Morse";
    public string SupportedCharacters => "A-Z, 0-9, espacio (palabras separadas por /)";

    private static readonly Dictionary<char, string> CharToMorse = new()
    {
        { 'A', ".-" },    { 'B', "-..." },  { 'C', "-.-." },
        { 'D', "-.." },   { 'E', "." },     { 'F', "..-." },
        { 'G', "--." },   { 'H', "...." },  { 'I', ".." },
        { 'J', ".---" },  { 'K', "-.-" },   { 'L', ".-.." },
        { 'M', "--" },    { 'N', "-." },    { 'O', "---" },
        { 'P', ".--." },  { 'Q', "--.-" },  { 'R', ".-." },
        { 'S', "..." },   { 'T', "-" },     { 'U', "..-" },
        { 'V', "...-" },  { 'W', ".--" },   { 'X', "-..-" },
        { 'Y', "-.--" },  { 'Z', "--.." },
        { '0', "-----" }, { '1', ".----" }, { '2', "..---" },
        { '3', "...--" }, { '4', "....-" }, { '5', "....." },
        { '6', "-...." }, { '7', "--..." }, { '8', "---.." },
        { '9', "----." },
    };

    private static readonly Dictionary<string, char> MorseToChar;

    static MorseCipherAlgorithm()
    {
        MorseToChar = new Dictionary<string, char>();
        foreach (var kvp in CharToMorse)
            MorseToChar[kvp.Value] = kvp.Key;
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
                sb.Append(" / ");
                first = true;
                continue;
            }

            var upper = char.ToUpperInvariant(c);
            if (CharToMorse.TryGetValue(upper, out var morse))
            {
                if (!first)
                    sb.Append(' ');
                sb.Append(morse);
                first = false;
            }
            else
            {
                // no lo reconozco, lo copio tal cual
                if (!first)
                    sb.Append(' ');
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
        // Dividir por " / " para obtener palabras
        var words = input.Split(new[] { " / " }, StringSplitOptions.None);

        for (int w = 0; w < words.Length; w++)
        {
            if (w > 0) sb.Append(' ');

            var tokens = words[w].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (MorseToChar.TryGetValue(token, out var letter))
                    sb.Append(letter);
                else
                    sb.Append(token); // no lo reconozco, lo dejo como esta
            }
        }

        return sb.ToString();
    }
}
