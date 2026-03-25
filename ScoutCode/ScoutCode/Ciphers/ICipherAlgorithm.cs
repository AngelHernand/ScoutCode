namespace ScoutCode.Ciphers;

// Interfaz que todos los cifrados deben implementar
public interface ICipherAlgorithm
{
    string Encrypt(string input);
    string Decrypt(string input);
    string SupportedCharacters { get; }
    string DisplayName { get; }
}
