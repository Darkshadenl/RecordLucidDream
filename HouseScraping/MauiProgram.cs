using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using HouseScraping.Services;
using OpenAI;
using HouseScraping.ViewModels;
using Interfaces;
using HouseScraping.Services.CompiledServices.Audio;
using HouseScraping.Database;
using HouseScraping.Models.Settings;
using HouseScraping.Services.CompiledServices.AI;
using Microsoft.EntityFrameworkCore;
using HouseScraping.Views.Homescreen;

namespace HouseScraping;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
        => MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .ConfigureFonts()
            .ConfigureLogging()
            .AddAudio()
            .RegisterDatabaseServices()
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

    private static MauiAppBuilder RegisterDatabaseServices(this MauiAppBuilder builder)
    {
        builder.Services.AddDbContext<LucidDbContext>(options =>
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Join(path, "LucidDatabase.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        return builder;
    }

    private static MauiAppBuilder ConfigureLogging(this MauiAppBuilder builder)
    {
#if DEBUG
        builder.Services.AddLogging(_ =>
        {
            builder.Logging.AddDebug();
        });
#endif
        return builder;
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IAudioRecordedEventBus, AudioRecordedEventBus>();
        builder.Services.AddSingleton<IAudioRecordingService, AudioRecordingService>();
        builder.Services.AddSingleton<IWhisperService, WhisperService>();
        builder.Services.AddSingleton<ILLMService, LLMService>();

        builder.Services.AddSingleton<AppShell>();

        // compiled services
        builder.Services.AddSingleton<IAudioServices, AudioServices>();
        builder.Services.AddSingleton<IAiServices, AiServices>();

        builder.Services.AddSingleton(sp =>
        {
            var settingsService = sp.GetRequiredService<ISettingsService>();
            var apiKey = settingsService.Settings.ApiKeys.OpenAIKey
                ?? throw new InvalidOperationException("OpenAI API key not found in settings");
            return new OpenAIClient(apiKey);
        });

        return builder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<MainViewModel>();
        return builder;
    }

    private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<RecordAudioPage>();
        return builder;
    }
}
