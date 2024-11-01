using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using HouseScraping.Services;
using OpenAI;
using HouseScraping.ViewModels;

namespace HouseScraping;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		RegisterServices(builder.Services);

        return builder.Build();
	}

	private static void RegisterServices(IServiceCollection services)
    {
        // Core services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton(AudioManager.Current);

        // OpenAI integratie
        services.AddSingleton(sp =>
        {
            var settingsService = sp.GetRequiredService<ISettingsService>();
            var apiKey = settingsService.Settings.ApiKeys.OpenAIKey
                ?? throw new InvalidOperationException("OpenAI API key not found in settings");
            return new OpenAIClient(apiKey);
        });

        // Applicatie services
        services.AddSingleton<IAudioRecordingService, AudioRecordingService>();
        services.AddSingleton<IWhisperService, WhisperService>();
        services.AddSingleton<ILLMService, LLMService>();

        // ViewModels
        services.AddTransient<MainViewModel>();

        // Pages
        services.AddTransient<MainPage>();
    }
}
