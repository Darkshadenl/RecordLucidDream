using System;

namespace HouseScraping.Model;

public class AudioRecordingInfo
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public DateTime RecordedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public long FileSizeBytes { get; set; }
    public bool IsProcessed { get; set; }
    public string TranscriptionStatus { get; set; }
}

