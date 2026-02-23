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
			});

		// Servicios
		builder.Services.AddSingleton<ICipherService, CipherService>();

		// Pipeline de camara (por ahora placeholder)
		builder.Services.AddSingleton<ICameraPipeline, PlaceholderCameraPipeline>();
		builder.Services.AddSingleton<IImageSegmenter, PlaceholderImageSegmenter>();
		builder.Services.AddSingleton<ISymbolClassifier, PlaceholderSymbolClassifier>();

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
