namespace ScoutCode.Pipelines;

// Interfaz del pipeline de camara (todavia no implementado de verdad)
public interface ICameraPipeline
{
    Task<string> ProcessImageAsync(byte[] imageBytes);
}
