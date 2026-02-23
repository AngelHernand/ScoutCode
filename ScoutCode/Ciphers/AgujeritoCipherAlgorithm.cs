namespace ScoutCode.Ciphers;

// Agujerito: A<->O, G<->T, U<->I, J<->R, la E se queda igual.
// Simetrico
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
        // la E no se toca
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
