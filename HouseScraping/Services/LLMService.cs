namespace HouseScraping.Services;

using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

public class LLMService : Interfaces.ILLMService
{
    private readonly ChatClient _chatClient;

    public LLMService(OpenAIClient openAIClient)
    {
        _chatClient = openAIClient.GetChatClient("gpt-4o");
    }

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        var completion = await _chatClient.CompleteChatAsync(prompt);
        return completion.Value.Content[0].Text;
    }
}
