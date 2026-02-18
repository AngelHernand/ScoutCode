namespace ScoutCode.Ciphers;

/// <summary>
/// Interfaz base para todos los algoritmos de cifrado scout.
/// Cada clave scout debe implementar esta interfaz.
/// </summary>
public interface ICipherAlgorithm
{
    /// <summary>
    /// Cifra el texto de entrada usando el algoritmo específico.
    /// </summary>
    /// <param name="input">Texto plano a cifrar.</param>
    /// <returns>Texto cifrado.</returns>
    string Encrypt(string input);

    /// <summary>
    /// Descifra el texto de entrada usando el algoritmo específico.
    /// </summary>
    /// <param name="input">Texto cifrado a descifrar.</param>
    /// <returns>Texto plano descifrado.</returns>
    string Decrypt(string input);

    /// <summary>
    /// Caracteres soportados por este algoritmo.
    /// </summary>
    string SupportedCharacters { get; }

    /// <summary>
    /// Nombre legible del algoritmo.
    /// </summary>
    string DisplayName { get; }
}
