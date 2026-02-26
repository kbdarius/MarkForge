namespace MarkForge.App.Models;

public sealed class AppSettings
{
    public string? LastInputFilePath { get; set; }

    public string? LastOutputFolderPath { get; set; }

    public string? LastTemplatePath { get; set; }

    public bool IncludeTableOfContents { get; set; } = true;

    public bool UseLandscapeOrientation { get; set; }

    public string HighlightStyle { get; set; } = "yellow";

    public DateTimeOffset? LastSavedUtc { get; set; }
}
