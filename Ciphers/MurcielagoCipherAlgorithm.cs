using System.Text;

namespace ScoutCode.Ciphers;

/// <summary>
/// Cifrado Murciélago: M=0, U=1, R=2, C=3, I=4, E=5, L=6, A=7, G=8, O=9.
/// Cifrar: letras MURCIELAGO → dígitos. Descifrar: dígitos → letras.
/// Preserva case en descifrado. Todo lo demás se copia igual.
/// </summary>
public class MurcielagoCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Murciélago";
    public string SupportedCharacters => "M,U,R,C,I,E,L,A,G,O ↔ 0-9 (resto se mantiene igual)";

    private static readonly Dictionary<char, char> LetterToDigit = new()
    {
        { 'M', '0' }, { 'U', '1' }, { 'R', '2' },
        { 'C', '3' }, { 'I', '4' }, { 'E', '5' },
        { 'L', '6' }, { 'A', '7' }, { 'G', '8' },
        { 'O', '9' }
    };

    private static readonly Dictionary<char, char> DigitToLetter = new()
    {
        { '0', 'M' }, { '1', 'U' }, { '2', 'R' },
        { '3', 'C' }, { '4', 'I' }, { '5', 'E' },
        { '6', 'L' }, { '7', 'A' }, { '8', 'G' },
        { '9', 'O' }
    };

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            var upper = char.ToUpperInvariant(c);
            if (LetterToDigit.TryGetValue(upper, out var digit))
                sb.Append(digit);
            else
                sb.Append(c); // conservar tal cual (espacios, signos, letras no-MURCIELAGO)
        }
        return sb.ToString();
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            if (DigitToLetter.TryGetValue(c, out var letter))
                sb.Append(letter); // dígitos → siempre mayúscula
            else
                sb.Append(c);
        }
        return sb.ToString();
    }
}
