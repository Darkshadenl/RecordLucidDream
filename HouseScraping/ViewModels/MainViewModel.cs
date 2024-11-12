using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HouseScraping.Model;
using HouseScraping.Services.CompiledServices.AI;
using HouseScraping.Services.CompiledServices.Audio;
using Interfaces;
using Plugin.Maui.Audio;

namespace HouseScraping.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAudioManager _audioManager;
    private readonly IAudioRecordingService _audioRecordingService;
    private readonly IWhisperService _whisperService;
    private readonly ILLMService _llmService;
    

    public ObservableCollection<IAudioRecordingInfo> AudioFiles { get; set; }

    [ObservableProperty]
    private bool isRecording = false;

    [ObservableProperty]
    private string buttonText = "Start Recording";


    public MainViewModel(
        IAudioServices audioServices,
        IAiServices aiServices,
        IAudioRecordedEventBus audioRecordedEventBus)
    {
        _audioManager = audioServices.Manager;
        _audioRecordingService = audioServices.Recording;    
        _whisperService = aiServices.WhisperService;
        _llmService = aiServices.LLMService;

        AudioFiles = [];
        LoadAudioFiles();

        audioRecordedEventBus.Subscribe<IAudioRecordingInfo>(OnNewRecordingCreated);
    }

    private void OnNewRecordingCreated(IAudioRecordingInfo newRecording)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AudioFiles.Insert(0, newRecording);
            OnPropertyChanged(nameof(AudioFiles));
        });
    }

    [RelayCommand]
    private async Task RecordAudio()
    {
        try
        {
            if (!IsRecording)
            {
                if (await _audioRecordingService.StartRecordingAsync())
                {
                    IsRecording = true;
                    ButtonText = "Recording...";
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fout", 
                        "Kon de opname niet starten. Controleer of de app toegang heeft tot de microfoon.", 
                        "OK");
                }
            }
            else
            {
                IsRecording = !await _audioRecordingService.StopRecordingAsync();

                if (IsRecording == true) {
                    IsRecording = false;
                    await Shell.Current.DisplayAlert("Error", "Er ging iets fout met het stoppen van de opname", "OK");
                }
                ButtonText = "Start Recording";
            }
            OnPropertyChanged(nameof(ButtonText));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void LoadAudioFiles()
    {
        AudioFiles.Clear();
        string cacheDir = FileSystem.CacheDirectory;
        var files = Directory.GetFiles(cacheDir, "recording_*.m4a");

        foreach (var file in files)
        {
            AudioFiles.Add(new Models.AudioRecordingInfo
            {
                FileName = Path.GetFileName(file),
                FilePath = file
            });
        }
    }

    [RelayCommand]
    private void PlayAudio(string filePath)
    {
        var player = _audioManager.CreatePlayer(filePath);
        player.Play();
    }

    [RelayCommand]
    private void DeleteAudio(IAudioRecordingInfo audioRecordingInfo)
    {
        if (File.Exists(audioRecordingInfo.FilePath))
            File.Delete(audioRecordingInfo.FilePath);

        AudioFiles.Remove(audioRecordingInfo);
        OnPropertyChanged(nameof(AudioFiles));
    }

    [RelayCommand]
    private async Task TranscribeAudio(IAudioRecordingInfo audioRecordingInfo)
    {
        var resultText = await _whisperService.TranscribeAudioAsync(audioRecordingInfo.FilePath);
        System.Console.WriteLine(resultText);
    }
}

