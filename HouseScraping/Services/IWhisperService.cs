using System;

namespace HouseScraping.Services;

public interface IWhisperService
{
    Task<string> TranscribeAudioAsync(string filePath);
}



