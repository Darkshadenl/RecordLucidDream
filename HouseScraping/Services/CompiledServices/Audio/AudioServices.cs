using System;
using Interfaces;
using Plugin.Maui.Audio;

namespace HouseScraping.Services.CompiledServices.Audio;

public class AudioServices : IAudioServices
{
    public IAudioRecordingService Recording { get; }
    public IAudioManager Manager { get; }

    public AudioServices(
        IAudioRecordingService recording,
        IAudioManager manager)
    {
        Recording = recording;
        Manager = manager;
    }

}
