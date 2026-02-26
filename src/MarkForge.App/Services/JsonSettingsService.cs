using System.Text.Json;
using System.IO;
using MarkForge.App.Models;

namespace MarkForge.App.Services;

public sealed class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public JsonSettingsService(string? settingsRoot = null)
    {
        var basePath = settingsRoot
            ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        SettingsFilePath = Path.Combine(basePath, "MarkForge", "settings.json");
    }

    public string SettingsFilePath { get; }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new AppSettings();
        }

        await using var stream = File.OpenRead(SettingsFilePath);
        try
        {
            var settings = await JsonSerializer.DeserializeAsync<AppSettings>(
                stream,
                SerializerOptions,
                cancellationToken);

            return Normalize(settings ?? new AppSettings());
        }
        catch (JsonException)
        {
            return new AppSettings();
        }
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(SettingsFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var normalized = Normalize(settings);
        normalized.LastSavedUtc = DateTimeOffset.UtcNow;

        await using var stream = File.Create(SettingsFilePath);
        await JsonSerializer.SerializeAsync(stream, normalized, SerializerOptions, cancellationToken);
    }

    private static AppSettings Normalize(AppSettings settings)
    {
        settings.HighlightStyle = string.IsNullOrWhiteSpace(settings.HighlightStyle)
            ? "yellow"
            : settings.HighlightStyle;
        return settings;
    }
}
