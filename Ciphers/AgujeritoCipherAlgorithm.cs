namespace ScoutCode.Ciphers;

/// <summary>
/// Cifrado Agujerito: A↔O, G↔T, U↔I, J↔R, E=E (se mantiene).
/// Simétrico. Preserva case. Todo lo demás se copia igual.
///
/// AGUJERITO:
/// A↔O, G↔T, U↔I, J↔R, E=E
/// </summary>
public class AgujeritoCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Agujerito";
    public string SupportedCharacters => "A↔O, G↔T, U↔I, J↔R, E=E (resto se mantiene igual)";

    private static readonly Dictionary<char, char> SwapMap = new()
    {
        { 'A', 'O' }, { 'O', 'A' },
        { 'G', 'T' }, { 'T', 'G' },
        { 'U', 'I' }, { 'I', 'U' },
        { 'J', 'R' }, { 'R', 'J' },
        // E ↔ E: no hace falta, se mantiene
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
