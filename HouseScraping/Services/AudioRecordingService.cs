using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Maui.Audio;

namespace HouseScraping.Services;

public class AudioRecordingService : IAudioRecordingService
{
    private readonly IAudioRecorder audioRecorder;
    private string filePath;

    public AudioRecordingService(IAudioManager audioManager)
    {
        audioRecorder = audioManager.CreateRecorder();
        filePath = string.Empty;
    }

   public async Task StartRecordingAsync()
    {
        if (!audioRecorder.IsRecording)
        {
            await audioRecorder.StartAsync();
        }
    }

    public async Task StopRecordingAsync()
    {
        if (audioRecorder.IsRecording)
        {
           var recordingResult = await audioRecorder.StopAsync();
            
            string fileName = $"recording_{DateTime.Now:yyyyMMddHHmmss}.wav";
            string cacheDir = FileSystem.CacheDirectory;
            filePath = Path.Combine(cacheDir, fileName);

            using (var audioStream = recordingResult.GetAudioStream())
            using (var fileStream = File.Create(filePath))
            {
                await audioStream.CopyToAsync(fileStream);
            }
        }
    }

    public string GetAudioFilePath() => filePath;

}
