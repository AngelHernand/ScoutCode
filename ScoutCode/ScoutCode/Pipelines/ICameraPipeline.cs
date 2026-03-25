using ScoutCode.Models;

namespace ScoutCode.Pipelines;

public interface ICameraPipeline
{
    Task<string> ProcessImageAsync(byte[] imageBytes);

    /// <summary>
    /// Procesa una imagen de símbolos cifrados y devuelve el formato intermedio
    /// (ej. "GATO:h,o,l,a") que puede ser descifrado por el CipherService.
    /// </summary>
    Task<string> ProcessSymbolicImageAsync(byte[] imageBytes, CipherType cipherType);
}
