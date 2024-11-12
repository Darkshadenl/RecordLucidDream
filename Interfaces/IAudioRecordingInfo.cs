using System;

namespace Interfaces;

public interface IAudioRecordingInfo
{
    int Id { get; set; }
    string FilePath { get; set; }
    string FileName { get; set; }
    DateTime RecordedAt { get; set; }
    long FileSizeBytes { get; set; }
    bool IsProcessed { get; set; }
    string TranscriptionStatus { get; set; }
}
