using System;
using HouseScraping.Model.Settings;
using Newtonsoft.Json;


namespace HouseScraping.Services;

public class SettingsService : ISettingsService
{
    public AppSettings Settings { get; private set; }

    public SettingsService()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
        using var reader = new StreamReader(stream);

        string json = reader.ReadToEnd();
        Settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? 
                new AppSettings() { ApiKeys = new ApiKeys() { OpenAIKey = string.Empty } };
    }

}
