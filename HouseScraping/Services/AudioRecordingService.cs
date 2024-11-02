using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Maui.Audio;
using HouseScraping.Helpers;

namespace HouseScraping.Services;

public class AudioRecordingService : IAudioRecordingService
{
    private readonly IAudioRecorder audioRecorder;
    private string filePath;

    public AudioRecordingService(IAudioManager audioManager)
    {
        audioRecorder = audioManager.CreateRecorder();

        System.Console.WriteLine(audioRecorder.CanRecordAudio);

        filePath = string.Empty;
    }

   public async Task<bool> StartRecordingAsync()
    {
        try
        {
            var status = await PermissionHelper.CheckAndRequestMicrophonePermission();
            
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Microfoon permissie niet verleend");
                return false;
            }

            if (!audioRecorder.IsRecording)
            {
                await audioRecorder.StartAsync();
                Console.WriteLine("Opname gestart");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij starten opname: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StopRecordingAsync()
    {
        try
        {
            if (audioRecorder.IsRecording)
            {
                IAudioSource recordingResult = await audioRecorder.StopAsync();
                
                string fileName = $"recording_{DateTime.Now:yyyyMMddHHmmss}.m4a";
                string cacheDir = FileSystem.CacheDirectory;
                filePath = Path.Combine(cacheDir, fileName);

                using (var audioStream = recordingResult.GetAudioStream())
                using (var fileStream = File.Create(filePath))
                {
                    var player = AudioManager.Current.CreatePlayer(audioStream);
                    await audioStream.CopyToAsync(fileStream);
                }
                return true;
            } else {
                return false;
            }
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw;
        }
    }

    public string GetAudioFilePath() => filePath;

}
