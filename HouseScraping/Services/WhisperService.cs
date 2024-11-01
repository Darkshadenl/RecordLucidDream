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

        // Voer de transcriptie uit
        AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(filePath);
        Console.WriteLine($"Transcriptie voltooid: {transcription.Text}");

        return transcription.Text;
    }
}
