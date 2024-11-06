using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseScraping.Model;

namespace HouseScraping.Services;

public interface IAudioRecordingService
{
    Task<bool> StartRecordingAsync();
    Task<bool> StopRecordingAsync();
    string GetAudioFilePath();
    event EventHandler<AudioRecordingInfo> NewRecordingCreated;
}