namespace Interfaces;

public interface IAudioRecordingService
{
    Task<bool> StartRecordingAsync();
    Task<bool> StopRecordingAsync();
    string GetAudioFilePath();
    
}