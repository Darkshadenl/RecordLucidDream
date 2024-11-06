using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HouseScraping.Model;
using HouseScraping.Services;
using Plugin.Maui.Audio;

namespace HouseScraping.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAudioRecordingService _audioRecordingService;
    private readonly IWhisperService _whisperService;
    private readonly ILLMService _llmService;
    private readonly IAudioManager _audioManager;

    public ICommand RecordCommand { get; }
    public ICommand PlayAudioCommand { get; }
    public ICommand DeleteAudioCommand { get; }

    public ObservableCollection<AudioRecordingInfo> AudioFiles { get; set; }

    [ObservableProperty]
    private bool isRecording = false;

    [ObservableProperty]
    private string buttonText = "Start Recording";


    public MainViewModel(
        IAudioRecordingService audioService,
        IWhisperService whisperService,
        ILLMService llmService,
        IAudioManager audioManager)
    {
        _audioRecordingService = audioService;
        _whisperService = whisperService;
        _llmService = llmService;
        _audioManager = audioManager;

        AudioFiles = new ObservableCollection<AudioRecordingInfo>();
        LoadAudioFiles();

        _audioRecordingService.NewRecordingCreated += OnNewRecordingCreated;

        RecordCommand = new Command(async () => await ToggleRecordingAsync());
        PlayAudioCommand = new Command<string>(PlayAudio);
        DeleteAudioCommand = new Command<AudioRecordingInfo>(DeleteAudio);
    }

    private void OnNewRecordingCreated(object? sender, AudioRecordingInfo newRecording)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AudioFiles.Insert(0, newRecording);
            OnPropertyChanged(nameof(AudioFiles));
        });
    }

    [RelayCommand]
    private async Task ToggleRecordingAsync()
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

                // Transcribe the audio
                string audioPath = _audioRecordingService.GetAudioFilePath();
                string transcription = await _whisperService.TranscribeAudioAsync(audioPath);
                
                // Process with LLM if needed
                // var response = await _llmService.GenerateResponseAsync(transcription);
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
            AudioFiles.Add(new AudioRecordingInfo
            {
                FileName = Path.GetFileName(file),
                FilePath = file
            });
        }
    }

    private void PlayAudio(string filePath)
    {
        var player = _audioManager.CreatePlayer(filePath);
        player.Play();
    }

    private void DeleteAudio(AudioRecordingInfo audioRecordingInfo)
    {
        if (File.Exists(audioRecordingInfo.FilePath))
            File.Delete(audioRecordingInfo.FilePath);

        AudioFiles.Remove(audioRecordingInfo);
        OnPropertyChanged(nameof(AudioFiles));
    }
}

