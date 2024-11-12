using HouseScraping.Model.Settings;

namespace HouseScraping.Services;

public interface ISettingsService
{
    AppSettings Settings { get; }
}
