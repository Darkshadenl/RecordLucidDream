using System;
using Interfaces;

namespace HouseScraping.Models;

public class AudioRecordingInfo : IAudioRecordingInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
    public long FileSizeBytes { get; set; }
    public bool IsProcessed { get; set; }
    public string TranscriptionStatus { get; set; } = string.Empty;
    public int Id { get; set; }
}
