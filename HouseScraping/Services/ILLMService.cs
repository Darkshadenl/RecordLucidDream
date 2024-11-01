using System;

namespace HouseScraping.Services;

public interface ILLMService
{
    Task<string> GenerateResponseAsync(string prompt);
}

