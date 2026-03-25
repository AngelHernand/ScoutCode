namespace ScoutCode.Ciphers;

// Baden-Powell: B<->P, A<->O, D<->W, N<->L. La E se queda igual.
public class BadenPowelCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Baden-Powell";
    public string SupportedCharacters => "B↔P, A↔O, D↔W, E=E, N↔L (resto se mantiene igual)";

    private static readonly Dictionary<char, char> SwapMap = new()
    {
        { 'B', 'P' }, { 'P', 'B' },
        { 'A', 'O' }, { 'O', 'A' },
        { 'D', 'W' }, { 'W', 'D' },
        // la E no se toca
        { 'N', 'L' }, { 'L', 'N' },
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
