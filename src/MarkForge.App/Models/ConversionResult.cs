namespace MarkForge.App.Models;

public sealed class ConversionResult
{
    public required string OutputDocxPath { get; init; }

    public required string DetailedLogPath { get; init; }
}
