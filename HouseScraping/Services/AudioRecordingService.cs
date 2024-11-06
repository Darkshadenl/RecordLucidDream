using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using HouseScraping.Helpers;
using Microsoft.Extensions.Logging;
using HouseScraping.Model;

namespace HouseScraping.Services;

public class AudioRecordingService : IAudioRecordingService
{
    private readonly IAudioRecorder audioRecorder;
    private string filePath;
    private readonly ILogger<AudioRecordingService> _logger;
    public event EventHandler<AudioRecordingInfo> NewRecordingCreated;

    public AudioRecordingService(ILogger<AudioRecordingService> logger, IAudioManager audioManager)
    {
        _logger = logger;
        audioRecorder = audioManager.CreateRecorder();
        Console.WriteLine(audioRecorder.CanRecordAudio);
        filePath = string.Empty;
    }

    public async Task<bool> StartRecordingAsync()
    {
        try
        {
            var status = await PermissionHelper.CheckAndRequestMicrophonePermission();

            if (status != PermissionStatus.Granted)
            {
                _logger.LogWarning("Microfoon permissie niet verleend");
                return false;
            }

            if (!audioRecorder.IsRecording)
            {
                await audioRecorder.StartAsync();
                _logger.LogInformation("Opname gestart");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fout bij starten opname");
            return false;
        }
    }

    public async Task<bool> StopRecordingAsync()
    {
        if (audioRecorder.IsRecording)
        {
            IAudioSource recordingResult = await audioRecorder.StopAsync();

            string fileName = $"recording_{DateTime.Now:yyyyMMddHHmmss}.m4a";
            _logger.LogInformation($"Opname opgeslagen in {filePath}");
            string cacheDir = FileSystem.CacheDirectory;
            filePath = Path.Combine(cacheDir, fileName);

            using var audioStream = recordingResult.GetAudioStream();

            var audioInfo = new AudioRecordingInfo
            {
                FilePath = filePath,
                FileName = fileName,
                RecordedAt = DateTime.Now,
                IsProcessed = false,
                TranscriptionStatus = "Not Started"
            };

            using (var fileStream = File.Create(filePath))
                await audioStream.CopyToAsync(fileStream);
            
            NewRecordingCreated?.Invoke(this, audioInfo);

            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetAudioFilePath() => filePath;

}

