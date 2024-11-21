using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HouseScraping.Services.CompiledServices.AI;
using HouseScraping.Services.CompiledServices.Audio;
using Interfaces;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace HouseScraping.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly IAudioManager _audioManager;
    private readonly IAudioRecordingService _audioRecordingService;
    private readonly IWhisperService _whisperService;
    private readonly ILLMService _llmService;
    private IDisposable? _currentPlayer;

    public ObservableCollection<IAudioRecordingInfo> AudioFiles { get; set; }

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private string _buttonText = "Start Recording";


    public MainViewModel(
        IAudioServices audioServices,
        IAiServices aiServices,
        IAudioRecordedEventBus audioRecordedEventBus,
        ILogger<MainViewModel> logger)
    {
        _logger = logger;
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
            try
            {
                AudioFiles.Insert(0, newRecording);
                OnPropertyChanged(nameof(AudioFiles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error was empty in {nameof(OnNewRecordingCreated)}");
            }
        });
    }

    private bool GetIsRecording() => !IsRecording;

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
                    ButtonText = "Recording... Click to stop recording.";
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
                bool stopped = await _audioRecordingService.StopRecordingAsync();
                IsRecording = !stopped;

                if (IsRecording) {
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
        try
        {
            AudioFiles.Clear();
            string cacheDir = FileSystem.CacheDirectory;

            if (Directory.Exists(cacheDir))
            {
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
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(LoadAudioFiles)} -> {ex.Message}");
        }
    }

    [RelayCommand]
    private void PlayAudio(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            _currentPlayer?.Dispose();
            _currentPlayer = _audioManager.CreatePlayer(filePath);
            (_currentPlayer as IAudioPlayer)?.Play();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing audio: {ex.Message}");
        }
    }

    [RelayCommand]
    private void DeleteAudio(IAudioRecordingInfo audioRecordingInfo)
    {
        if (File.Exists(audioRecordingInfo.FilePath))
            File.Delete(audioRecordingInfo.FilePath);

        AudioFiles.Remove(audioRecordingInfo);
        OnPropertyChanged(nameof(AudioFiles));
    }

    [RelayCommand (CanExecute = nameof(GetIsRecording))]
    private async Task TranscribeAudio(IAudioRecordingInfo audioRecordingInfo)
    {
        var resultText = await _whisperService.TranscribeAudioAsync(audioRecordingInfo.FilePath);
        System.Console.WriteLine(resultText);
        _logger.LogInformation(resultText);
    }
}

