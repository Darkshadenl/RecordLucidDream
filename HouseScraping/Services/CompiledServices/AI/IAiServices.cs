using System;
using Interfaces;

namespace HouseScraping.Services.CompiledServices.AI;

public interface IAiServices
{
    ILLMService LLMService { get; }
    IWhisperService WhisperService { get; }

}
