namespace HouseScraping.Services;

using System;
using System.IO;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Audio;

public class WhisperService : IWhisperService
{
    private readonly AudioClient _audioClient;

    public WhisperService(OpenAIClient openAIClient)
    {
        _audioClient = openAIClient.GetAudioClient("whisper-1");
    }

    public async Task<string> TranscribeAudioAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            throw new FileNotFoundException("Het audiobestand is niet gevonden.");
        }

        AudioTranscriptionOptions options = new AudioTranscriptionOptions
        {
            Language = "nl",
            Prompt = @"De transcriptie gaat over een droom die een van de gebruikers van onze app heeft gehad.
                Het zal worden gebruikt in zijn/haar droomdagboek om lucide dromen te bereiken."
        };

        AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(filePath, options);
        Console.WriteLine($"Transcriptie voltooid: {transcription.Text}");

        return transcription.Text;
    }
}

