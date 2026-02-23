using System.Text;

namespace ScoutCode.Ciphers;

// Clave -1: cada letra se mueve una posicion atras en el alfabeto español.
// A vuelve a Z. Descifrar es lo inverso (+1).
public class ShiftMinusOneCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Clave -1";
    public string SupportedCharacters => "A-Z + Ñ (cada letra → la anterior, A→Z)";

    public string Encrypt(string input) => Shift(input, -1);
    public string Decrypt(string input) => Shift(input, +1);

    private static string Shift(string input, int offset)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            int idx = CipherUtils.GetSpanishIndex(c);
            if (idx >= 0)
            {
                char shifted = CipherUtils.GetSpanishLetter(idx + offset);
                sb.Append(CipherUtils.PreserveCase(c, shifted));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
