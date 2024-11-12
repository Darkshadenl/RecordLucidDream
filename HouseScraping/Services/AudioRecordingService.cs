using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using HouseScraping.Helpers;
using Microsoft.Extensions.Logging;
using Interfaces;
using HouseScraping.Models;

namespace HouseScraping.Services;

public class AudioRecordingService : IAudioRecordingService
{
    private readonly IAudioRecorder audioRecorder;
    private string filePath;
    private readonly ILogger<IAudioRecordingService> _logger;
    private readonly IAudioRecordedEventBus _eventBus;

    public AudioRecordingService(ILogger<IAudioRecordingService> logger, IAudioManager audioManager,  IAudioRecordedEventBus eventBus)
    {
        _logger = logger;
        audioRecorder = audioManager.CreateRecorder();
        filePath = string.Empty;
        _eventBus = eventBus;
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
            string cacheDir = FileSystem.CacheDirectory;
            filePath = Path.Combine(cacheDir, fileName);

            using var audioStream = recordingResult.GetAudioStream();

            using (var fileStream = File.Create(filePath))
                await audioStream.CopyToAsync(fileStream);
            
            var fileInfo = new FileInfo(filePath);

            IAudioRecordingInfo audioInfo = new AudioRecordingInfo
            {
                FilePath = filePath,
                FileName = fileName,
                RecordedAt = DateTime.Now,
                IsProcessed = false,
                TranscriptionStatus = "Not Started",
                FileSizeBytes = fileInfo.Length,
            };

            _eventBus.Publish(audioInfo);

            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetAudioFilePath() => filePath;

}

