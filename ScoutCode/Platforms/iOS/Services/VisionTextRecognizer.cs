using Foundation;
using Microsoft.Extensions.Logging;
using ScoutCode.Services;
using UIKit;
using Vision;

namespace ScoutCode.Platforms.iOS.Services;

/// <summary>
/// Reconocimiento de texto en iOS usando Vision Framework (VNRecognizeTextRequest).
/// No requiere NuGet adicional: Vision viene incluido en iOS 13+.
/// </summary>
public class VisionTextRecognizer : ITextRecognitionService
{
    private readonly ILogger<VisionTextRecognizer> _logger;

    public bool IsAvailable => true;

    public VisionTextRecognizer(ILogger<VisionTextRecognizer> logger)
    {
        _logger = logger;
    }

    public Task<string> RecognizeTextAsync(byte[] imageBytes)
    {
        return Task.Run(() =>
        {
            try
            {
                var nsData = NSData.FromArray(imageBytes);
                var uiImage = UIImage.LoadFromData(nsData);
                if (uiImage?.CGImage == null)
                {
                    _logger.LogWarning("Vision: no se pudo decodificar la imagen");
                    return string.Empty;
                }

                string recognizedText = string.Empty;

                var request = new VNRecognizeTextRequest((req, error) =>
                {
                    if (error != null)
                    {
                        _logger.LogWarning("Vision callback error: {Error}", error.LocalizedDescription);
                        return;
                    }

                    var results = req.Results;
                    if (results == null || results.Length == 0)
                        return;

                    var lines = new List<string>();
                    foreach (var obs in results)
                    {
                        if (obs is VNRecognizedTextObservation textObs)
                        {
                            var candidates = textObs.TopCandidates(1);
                            if (candidates != null && candidates.Length > 0)
                                lines.Add(candidates[0].String);
                        }
                    }

                    recognizedText = string.Join("\n", lines);
                });

                // Accurate es mas lento pero mucho mejor para texto cifrado
                request.RecognitionLevel = VNRequestTextRecognitionLevel.Accurate;

                // NO autocorregir: el texto cifrado no es lenguaje natural
                request.UsesLanguageCorrection = false;

                var handler = new VNImageRequestHandler(uiImage.CGImage, new NSDictionary());
                handler.Perform(new VNRequest[] { request }, out var performError);

                if (performError != null)
                {
                    _logger.LogError("Vision Perform error: {Error}", performError.LocalizedDescription);
                    return string.Empty;
                }

                _logger.LogInformation("Vision: texto reconocido ({Length} chars)", recognizedText.Length);
                return recognizedText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vision: error en reconocimiento de texto");
                return string.Empty;
            }
        });
    }
}
