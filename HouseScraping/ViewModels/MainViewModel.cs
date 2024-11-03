using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HouseScraping.Services;

namespace HouseScraping.ViewModels;

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
}

