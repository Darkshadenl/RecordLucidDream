using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using HouseScraping.Services;
using OpenAI;
using HouseScraping.ViewModels;
using Interfaces;

namespace HouseScraping;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
        => MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .ConfigureFonts()
            .ConfigureLogging()
            .AddAudio()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterViews()
            .Build();

    private static MauiAppBuilder ConfigureFonts(this MauiAppBuilder builder)
    {
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        return builder;
    }

    private static MauiAppBuilder ConfigureLogging(this MauiAppBuilder builder)
    {
#if DEBUG
        builder.Services.AddLogging(loggingBuilder =>
        {
            builder.Logging.AddDebug();
        });
#endif
        return builder;
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IAudioRecordedEventBus, AudioRecordedEventBus>();
        builder.Services.AddSingleton<IAudioRecordingService, AudioRecordingService>();
        builder.Services.AddSingleton<IWhisperService, WhisperService>();
        builder.Services.AddSingleton<ILLMService, LLMService>();
        builder.Services.AddSingleton<AppShell>();
        
        builder.Services.AddSingleton(sp =>
        {
            var settingsService = sp.GetRequiredService<ISettingsService>();
            var apiKey = settingsService.Settings.ApiKeys.OpenAIKey
                ?? throw new InvalidOperationException("OpenAI API key not found in settings");
            return new OpenAIClient(apiKey);
        });

        return builder;
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<MainViewModel>();
        return builder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<MainPage>();
        return builder;
    }
}
