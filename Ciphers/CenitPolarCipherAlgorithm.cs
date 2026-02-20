using System.Text;

namespace ScoutCode.Ciphers;

/// <summary>
/// Cifrado Cenit-Polar: C↔P, E↔O, N↔L, I↔A, T↔R.
/// Simétrico: cifrar = descifrar. Preserva case. Deja todo lo demás igual.
/// </summary>
public class CenitPolarCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Cenit-Polar";
    public string SupportedCharacters => "C,E,N,I,T ↔ P,O,L,A,R (resto se mantiene igual)";

    private static readonly Dictionary<char, char> SwapMap = new()
    {
        { 'C', 'P' }, { 'P', 'C' },
        { 'E', 'O' }, { 'O', 'E' },
        { 'N', 'L' }, { 'L', 'N' },
        { 'I', 'A' }, { 'A', 'I' },
        { 'T', 'R' }, { 'R', 'T' },
    };

    public string Encrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
    public string Decrypt(string input) => CipherUtils.ApplyCharMap(input, SwapMap);
}
