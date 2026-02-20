namespace ScoutCode.Ciphers;

/// <summary>
/// Cifrado Parelinofo: P↔U, A↔F, R↔O, E↔N, L↔I (y sus inversos).
/// Simétrico. Preserva case. Todo lo demás se copia igual.
/// 
/// PARELINOFU:
/// P↔U, A↔F, R↔O, E↔N, L↔I
/// </summary>
public class ParelinofoCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Parelinofo";
    public string SupportedCharacters => "P↔U, A↔F, R↔O, E↔N, L↔I (resto se mantiene igual)";

    private static readonly Dictionary<char, char> SwapMap = new()
    {
        { 'P', 'U' }, { 'U', 'P' },
        { 'A', 'F' }, { 'F', 'A' },
        { 'R', 'O' }, { 'O', 'R' },
        { 'E', 'N' }, { 'N', 'E' },
        { 'L', 'I' }, { 'I', 'L' },
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
