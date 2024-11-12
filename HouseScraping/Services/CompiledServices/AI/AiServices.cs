using System;
using Interfaces;

namespace HouseScraping.Services.CompiledServices.AI;

public class AiServices : IAiServices
{
    public ILLMService LLMService { get; }

    public IWhisperService WhisperService { get; }

    public AiServices(ILLMService lLMService, IWhisperService whisperService)
    {
        LLMService = lLMService;
        WhisperService = whisperService;
    }
}
