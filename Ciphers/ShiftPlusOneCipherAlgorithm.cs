using System.Text;

namespace ScoutCode.Ciphers;

// Clave +1: cada letra se mueve una posicion adelante en el alfabeto español.
// Z vuelve a A. Descifrar es lo inverso (-1).
public class ShiftPlusOneCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Clave +1";
    public string SupportedCharacters => "A-Z + Ñ (cada letra → la siguiente, Z→A)";

    public string Encrypt(string input) => Shift(input, +1);
    public string Decrypt(string input) => Shift(input, -1);

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
                sb.Append(c); // no es letra, lo dejo
            }
        }
        return sb.ToString();
    }
}
