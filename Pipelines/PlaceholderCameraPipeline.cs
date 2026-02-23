namespace ScoutCode.Pipelines;

// Placeholder, por ahora solo devuelve un mensaje
public class PlaceholderCameraPipeline : ICameraPipeline
{
    public Task<string> ProcessImageAsync(byte[] imageBytes)
    {
        return Task.FromResult(
            "Funcionalidad en desarrollo.\n\n" +
            "Cuando este listo va a usar OpenCV para segmentar " +
            "y ONNX para clasificar los simbolos.");
    }
}
