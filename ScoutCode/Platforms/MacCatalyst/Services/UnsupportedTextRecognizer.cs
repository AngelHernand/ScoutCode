using ScoutCode.Services;

namespace ScoutCode.Platforms.MacCatalyst.Services;

/// <summary>
/// Placeholder para macOS Catalyst: el OCR no esta soportado en esta plataforma todavia.
/// IsAvailable retorna false, asi que la UI no muestra los controles de OCR.
/// </summary>
public class UnsupportedTextRecognizer : ITextRecognitionService
{
    public bool IsAvailable => false;

    public Task<string> RecognizeTextAsync(byte[] imageBytes)
    {
        throw new NotSupportedException(
            "El reconocimiento de texto no esta disponible en macOS. " +
            "Usa un dispositivo Android o iOS.");
    }
}
