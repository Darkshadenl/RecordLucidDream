using System;

namespace Interfaces;

public interface IWhisperService
{
    Task<string> TranscribeAudioAsync(string filePath);
}



