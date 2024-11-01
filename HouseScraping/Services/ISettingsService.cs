using System;
using HouseScraping.Model.Settings;
using Newtonsoft.Json;

namespace HouseScraping.Services;

public interface ISettingsService
{
    AppSettings Settings { get; }
}
