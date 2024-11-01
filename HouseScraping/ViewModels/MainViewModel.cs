namespace HouseScraping.ViewModels;

using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using HouseScraping.Services;

public partial class MainViewModel : BaseViewModel
{
    private readonly IAudioRecordingService _audioRecordingService;
    private readonly IWhisperService _whisperService;
    private readonly ILLMService _llmService;

    public ICommand RecordCommand { get; }

    [ObservableProperty]
    private bool isRecording = false;

    [ObservableProperty]
    private string buttonText = "Start Recording";


    public MainViewModel(
        IAudioRecordingService audioService,
        IWhisperService whisperService,
        ILLMService llmService)
    {
        _audioRecordingService = audioService;
        _whisperService = whisperService;
        _llmService = llmService;

        RecordCommand = new Command(async () => await ToggleRecordingAsync());
    }

    private async Task ToggleRecordingAsync()
    {
        try
        {
            if (!IsRecording)
            {
                await _audioRecordingService.StartRecordingAsync();
                IsRecording = true;
                ButtonText = "Recording...";
            }
            else
            {
                await _audioRecordingService.StopRecordingAsync();
                IsRecording = false;
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
            throw ex;
            // await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}

