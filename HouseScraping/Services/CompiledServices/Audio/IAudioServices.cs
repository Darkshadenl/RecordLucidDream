using System;
using Interfaces;
using Plugin.Maui.Audio;

namespace HouseScraping.Services.CompiledServices.Audio;

public interface IAudioServices
{
    IAudioRecordingService Recording { get; }
    IAudioManager Manager { get; }
}
