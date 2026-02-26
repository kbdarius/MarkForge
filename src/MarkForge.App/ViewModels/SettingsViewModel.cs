using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarkForge.App.Services;

namespace MarkForge.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        settingsFilePath = _settingsService.SettingsFilePath;
        settingsSummary = "No settings snapshot loaded yet.";
    }

    [ObservableProperty]
    private string settingsFilePath;

    [ObservableProperty]
    private string settingsSummary;

    [RelayCommand]
    private async Task Refresh()
    {
        var settings = await _settingsService.LoadAsync();
        SettingsSummary =
            $"Last Input File: {settings.LastInputFilePath ?? "(not set)"}{Environment.NewLine}" +
            $"Last Output Folder: {settings.LastOutputFolderPath ?? "(not set)"}{Environment.NewLine}" +
            $"Last Template File: {settings.LastTemplatePath ?? "(not set)"}{Environment.NewLine}" +
            $"Lua Filters Folder: {settings.LuaFiltersDirectoryPath ?? "(not set)"}{Environment.NewLine}" +
            $"Include TOC: {settings.IncludeTableOfContents}{Environment.NewLine}" +
            $"Landscape: {settings.UseLandscapeOrientation}{Environment.NewLine}" +
            $"Highlight Style: {settings.HighlightStyle}{Environment.NewLine}" +
            $"Last Saved (UTC): {(settings.LastSavedUtc?.ToString("u") ?? "(never)")}";
    }
}
