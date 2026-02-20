namespace ScoutCode.Ciphers;

/// <summary>
/// Cifrado Baden-Powell: B↔P, A↔O, D↔W, E↔E (se mantiene), N↔L.
/// Simétrico. Preserva case. Todo lo demás se copia igual.
/// </summary>
public class BadenPowelCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Baden-Powell";
    public string SupportedCharacters => "B↔P, A↔O, D↔W, E=E, N↔L (resto se mantiene igual)";

    private static readonly Dictionary<char, char> SwapMap = new()
    {
        { 'B', 'P' }, { 'P', 'B' },
        { 'A', 'O' }, { 'O', 'A' },
        { 'D', 'W' }, { 'W', 'D' },
        // E ↔ E: no hace falta agregarlo, se mantiene igual
        { 'N', 'L' }, { 'L', 'N' },
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
