namespace ScoutCode.Services;

/// <summary>
/// Interfaz compartida para reconocimiento de texto (OCR).
/// Cada plataforma tiene su propia implementación:
///   Android → ML Kit Text Recognition
///   iOS     → Vision Framework (VNRecognizeTextRequest)
///   Otras   → UnsupportedTextRecognizer (IsAvailable = false)
/// </summary>
public interface ITextRecognitionService
{
    /// <summary>Procesa los bytes de una imagen y devuelve el texto reconocido.</summary>
    Task<string> RecognizeTextAsync(byte[] imageBytes);

    /// <summary>Indica si la plataforma actual soporta OCR.</summary>
    bool IsAvailable { get; }
}
