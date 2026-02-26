namespace MarkForge.App.Models;

public sealed class ConversionRequest
{
    public required string InputFilePath { get; init; }

    public required string OutputFolderPath { get; init; }

    public string? ReferenceTemplatePath { get; init; }

    public bool IncludeTableOfContents { get; init; }

    public bool UseLandscapeOrientation { get; init; }

    public required string HighlightStyle { get; init; }
}
