using ScoutCode.Models;

namespace ScoutCode.Services;

/// <summary>
/// Servicio principal para procesar cifrados scout.
/// Actúa como fachada que enruta al algoritmo correcto según el tipo.
/// </summary>
public interface ICipherService
{
    /// <summary>
    /// Procesa una operación de cifrado/descifrado.
    /// </summary>
    /// <param name="type">Tipo de cifrado scout.</param>
    /// <param name="operation">Cifrar o descifrar.</param>
    /// <param name="input">Texto de entrada.</param>
    /// <returns>Resultado del procesamiento.</returns>
    string Process(CipherType type, OperationMode operation, string input);

    /// <summary>
    /// Obtiene la descripción de caracteres soportados para un cifrado.
    /// </summary>
    string GetSupportedCharacters(CipherType type);

    /// <summary>
    /// Obtiene la lista de definiciones de cifrados disponibles.
    /// </summary>
    List<CipherDefinition> GetAvailableCiphers();
}
