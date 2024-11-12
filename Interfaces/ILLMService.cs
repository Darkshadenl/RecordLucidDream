using System;

namespace Interfaces;

public interface ILLMService
{
    Task<string> GenerateResponseAsync(string prompt);
}

