using MarkForge.App.Models;

namespace MarkForge.App.Services;

public interface ISettingsService
{
    string SettingsFilePath { get; }

    Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
