using ScoutCode.Models;

namespace ScoutCode.Pipelines;

// Placeholder para plataformas sin soporte de pipeline de símbolos
public class PlaceholderCameraPipeline : ICameraPipeline
{
    public Task<string> ProcessImageAsync(byte[] imageBytes)
    {
        return Task.FromResult(
            "Funcionalidad en desarrollo.\n\n" +
            "Cuando este listo va a usar OpenCV para segmentar " +
            "y ONNX para clasificar los simbolos.");
    }

    public Task<string> ProcessSymbolicImageAsync(byte[] imageBytes, CipherType cipherType)
    {
        return Task.FromResult("Error: reconocimiento de símbolos no disponible en esta plataforma.");
    }
}
