namespace ScoutCode.Ciphers;

// Dametupico: D<->O, A<->C, M<->I, E<->P, T<->U
// Simetrico
public class DametupicoCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Dametupico";
    public string SupportedCharacters => "D↔O, A↔C, M↔I, E↔P, T↔U (resto se mantiene igual)";

    private static readonly Dictionary<char, char> SwapMap = new()
    {
        { 'D', 'O' }, { 'O', 'D' },
        { 'A', 'C' }, { 'C', 'A' },
        { 'M', 'I' }, { 'I', 'M' },
        { 'E', 'P' }, { 'P', 'E' },
        { 'T', 'U' }, { 'U', 'T' },
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
