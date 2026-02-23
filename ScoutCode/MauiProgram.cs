using Microsoft.Extensions.Logging;
using ScoutCode.Pipelines;
using ScoutCode.Services;
using ScoutCode.ViewModels;
using ScoutCode.Views;

namespace ScoutCode;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Inter-Regular.ttf", "InterRegular");
				fonts.AddFont("Inter-Medium.ttf", "InterMedium");
				fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
				fonts.AddFont("Inter-Bold.ttf", "InterBold");
				fonts.AddFont("Inter-ExtraBold.ttf", "InterExtraBold");
				fonts.AddFont("JetBrainsMono-Regular.ttf", "JetBrainsMonoRegular");
			});

		// Servicios
		builder.Services.AddSingleton<ICipherService, CipherService>();

		// Pipeline de camara (por ahora placeholder para Pigpen/simbolos)
		builder.Services.AddSingleton<ICameraPipeline, PlaceholderCameraPipeline>();
		builder.Services.AddSingleton<IImageSegmenter, PlaceholderImageSegmenter>();
		builder.Services.AddSingleton<ISymbolClassifier, PlaceholderSymbolClassifier>();

		// OCR: reconocimiento de texto por plataforma
#if ANDROID
		builder.Services.AddSingleton<ITextRecognitionService, ScoutCode.Platforms.Android.Services.MlKitTextRecognizer>();
#elif IOS
		builder.Services.AddSingleton<ITextRecognitionService, ScoutCode.Platforms.iOS.Services.VisionTextRecognizer>();
#elif WINDOWS
		builder.Services.AddSingleton<ITextRecognitionService, ScoutCode.Platforms.Windows.Services.UnsupportedTextRecognizer>();
#elif MACCATALYST
		builder.Services.AddSingleton<ITextRecognitionService, ScoutCode.Platforms.MacCatalyst.Services.UnsupportedTextRecognizer>();
#endif

		// ViewModels
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<CipherDetailViewModel>();

		// Views
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<CipherDetailPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
