using Android.Gms.Extensions;
using Android.Graphics;
using Microsoft.Extensions.Logging;
using ScoutCode.Services;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;

using MlKitText = Xamarin.Google.MLKit.Vision.Text.Text;

namespace ScoutCode.Platforms.Android.Services;

/// <summary>
/// Reconocimiento de texto en Android usando Google ML Kit Text Recognition (Latin).
/// NuGet: Xamarin.Google.MLKit.TextRecognition (solo Android).
/// </summary>
public class MlKitTextRecognizer : ITextRecognitionService
{
    private readonly ILogger<MlKitTextRecognizer> _logger;

    public bool IsAvailable => true;

    public MlKitTextRecognizer(ILogger<MlKitTextRecognizer> logger)
    {
        _logger = logger;
    }

    public async Task<string> RecognizeTextAsync(byte[] imageBytes)
    {
        try
        {
            var bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
            if (bitmap == null)
            {
                _logger.LogWarning("ML Kit: no se pudo decodificar la imagen a Bitmap");
                return string.Empty;
            }

            var inputImage = InputImage.FromBitmap(bitmap, 0);
            var recognizer = TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions);

            // Android.Gms.Extensions permite hacer await sobre un Android.Gms.Tasks.Task
            var result = await recognizer.Process(inputImage) as MlKitText;

            bitmap.Recycle();
            recognizer.Close();

            var text = result?.GetText() ?? string.Empty;

            _logger.LogInformation("ML Kit: texto reconocido ({Length} chars)", text.Length);
            return text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ML Kit: error en reconocimiento de texto");
            return string.Empty;
        }
    }
}
