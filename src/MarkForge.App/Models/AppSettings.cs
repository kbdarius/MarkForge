namespace MarkForge.App.Models;

public sealed class AppSettings
{
    public string? LastInputFilePath { get; set; }

    public string? LastOutputFolderPath { get; set; }

    public string? LastTemplatePath { get; set; }

    public string? LuaFiltersDirectoryPath { get; set; }

    public bool EnableOnlineMermaidRendering { get; set; } = true;

    public string TableStyleProfile { get; set; } = "template-default";

    public bool IncludeTableOfContents { get; set; } = true;

    public bool UseLandscapeOrientation { get; set; }

    public string HighlightStyle { get; set; } = "pygments";

    public DateTimeOffset? LastSavedUtc { get; set; }
}
