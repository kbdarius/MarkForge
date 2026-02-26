using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarkForge.App.Models;
using MarkForge.App.Services;

namespace MarkForge.App.ViewModels;

public partial class ConvertViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IConversionService _conversionService;
    private bool _hasUserCustomizedOutputFolder;

    public ConvertViewModel(
        ISettingsService settingsService,
        IFileDialogService fileDialogService,
        IConversionService conversionService)
    {
        _settingsService = settingsService;
        _fileDialogService = fileDialogService;
        _conversionService = conversionService;

        statusMessage = "Ready.";
    }

    public IReadOnlyList<string> HighlightStyles { get; } = new[]
    {
        "pygments",
        "tango",
        "kate",
        "monochrome",
        "none"
    };

    public ObservableCollection<string> LogEntries { get; } = new();

    public string SettingsFilePath => _settingsService.SettingsFilePath;

    [ObservableProperty]
    private string inputFilePath = string.Empty;

    [ObservableProperty]
    private string outputFolderPath = string.Empty;

    [ObservableProperty]
    private string referenceTemplatePath = string.Empty;

    [ObservableProperty]
    private string luaFiltersDirectoryPath = string.Empty;

    [ObservableProperty]
    private bool includeTableOfContents = true;

    [ObservableProperty]
    private bool useLandscapeOrientation;

    [ObservableProperty]
    private string highlightStyle = "pygments";

    [ObservableProperty]
    private string statusMessage;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string lastOutputDocxPath = string.Empty;

    [ObservableProperty]
    private string lastDetailedLogPath = string.Empty;

    [RelayCommand]
    private void BrowseInputFile()
    {
        var selected = _fileDialogService.PickMarkdownFile(InputFilePath);
        if (!string.IsNullOrWhiteSpace(selected))
        {
            InputFilePath = selected;
        }
    }

    [RelayCommand]
    private void BrowseOutputFolder()
    {
        var selected = _fileDialogService.PickFolder(OutputFolderPath);
        if (!string.IsNullOrWhiteSpace(selected))
        {
            OutputFolderPath = selected;
        }
    }

    [RelayCommand]
    private void BrowseTemplateFile()
    {
        var selected = _fileDialogService.PickReferenceTemplate(ReferenceTemplatePath);
        if (!string.IsNullOrWhiteSpace(selected))
        {
            ReferenceTemplatePath = selected;
        }
    }

    [RelayCommand]
    private void BrowseLuaFiltersFolder()
    {
        var selected = _fileDialogService.PickFolder(LuaFiltersDirectoryPath);
        if (!string.IsNullOrWhiteSpace(selected))
        {
            LuaFiltersDirectoryPath = selected;
        }
    }

    [RelayCommand]
    private async Task LoadSettings()
    {
        var settings = await _settingsService.LoadAsync();
        ApplySettings(settings);
        StatusMessage = "Settings loaded.";
        AppendLog($"Settings loaded from {SettingsFilePath}");
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        var settings = new AppSettings
        {
            LastInputFilePath = InputFilePath,
            LastOutputFolderPath = OutputFolderPath,
            LastTemplatePath = ReferenceTemplatePath,
            LuaFiltersDirectoryPath = LuaFiltersDirectoryPath,
            IncludeTableOfContents = IncludeTableOfContents,
            UseLandscapeOrientation = UseLandscapeOrientation,
            HighlightStyle = HighlightStyle
        };

        await _settingsService.SaveAsync(settings);
        StatusMessage = "Settings saved.";
        AppendLog($"Settings saved to {SettingsFilePath}");
    }

    [RelayCommand(CanExecute = nameof(CanRunConversion))]
    private async Task RunConversion()
    {
        IsBusy = true;
        LastOutputDocxPath = string.Empty;
        LastDetailedLogPath = string.Empty;
        StatusMessage = "Running conversion...";
        AppendLog("-----");
        AppendLog("Starting conversion run.");

        var request = new ConversionRequest
        {
            InputFilePath = InputFilePath,
            OutputFolderPath = OutputFolderPath,
            ReferenceTemplatePath = NormalizeNullablePath(ReferenceTemplatePath),
            LuaFiltersDirectoryPath = NormalizeNullablePath(LuaFiltersDirectoryPath),
            IncludeTableOfContents = IncludeTableOfContents,
            UseLandscapeOrientation = UseLandscapeOrientation,
            HighlightStyle = HighlightStyle
        };

        try
        {
            var progress = new Progress<string>(AppendLog);
            var result = await _conversionService.RunAsync(request, progress);
            LastOutputDocxPath = result.OutputDocxPath;
            LastDetailedLogPath = result.DetailedLogPath;
            StatusMessage = "Conversion completed.";
            AppendLog($"Output DOCX: {result.OutputDocxPath}");
            AppendLog($"Detailed log: {result.DetailedLogPath}");
            AppendLog("Run complete.");
        }
        catch (ConversionRunException ex)
        {
            LastDetailedLogPath = ex.DetailedLogPath ?? string.Empty;
            StatusMessage = "Conversion failed.";
            AppendLog($"ERROR: {ex.Message}");
            if (!string.IsNullOrWhiteSpace(ex.DetailedLogPath))
            {
                AppendLog($"Detailed log: {ex.DetailedLogPath}");
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Conversion failed.";
            AppendLog($"ERROR: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanRunConversion()
    {
        return !IsBusy
               && !string.IsNullOrWhiteSpace(InputFilePath)
               && !string.IsNullOrWhiteSpace(OutputFolderPath);
    }

    partial void OnInputFilePathChanged(string value)
    {
        var inputDirectory = TryGetInputDirectory(value);
        if (!string.IsNullOrWhiteSpace(inputDirectory)
            && (string.IsNullOrWhiteSpace(OutputFolderPath) || !_hasUserCustomizedOutputFolder)
            && !string.Equals(OutputFolderPath, inputDirectory, StringComparison.OrdinalIgnoreCase))
        {
            OutputFolderPath = inputDirectory;
            AppendLog($"Output folder defaulted to input directory: {inputDirectory}");
        }

        RunConversionCommand.NotifyCanExecuteChanged();
    }

    partial void OnOutputFolderPathChanged(string value)
    {
        var inputDirectory = TryGetInputDirectory(InputFilePath);
        _hasUserCustomizedOutputFolder =
            !string.IsNullOrWhiteSpace(value)
            && !string.IsNullOrWhiteSpace(inputDirectory)
            && !string.Equals(value, inputDirectory, StringComparison.OrdinalIgnoreCase);

        RunConversionCommand.NotifyCanExecuteChanged();
    }

    partial void OnIsBusyChanged(bool value)
    {
        RunConversionCommand.NotifyCanExecuteChanged();
    }

    private void ApplySettings(AppSettings settings)
    {
        InputFilePath = settings.LastInputFilePath ?? string.Empty;
        OutputFolderPath = settings.LastOutputFolderPath ?? string.Empty;
        ReferenceTemplatePath = settings.LastTemplatePath ?? string.Empty;
        LuaFiltersDirectoryPath = settings.LuaFiltersDirectoryPath ?? string.Empty;
        IncludeTableOfContents = settings.IncludeTableOfContents;
        UseLandscapeOrientation = settings.UseLandscapeOrientation;
        HighlightStyle = string.IsNullOrWhiteSpace(settings.HighlightStyle)
            ? "pygments"
            : settings.HighlightStyle;
    }

    private void AppendLog(string message)
    {
        LogEntries.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    private static string? TryGetInputDirectory(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var directory = Path.GetDirectoryName(path);
        return string.IsNullOrWhiteSpace(directory) ? null : directory;
    }

    private static string? NormalizeNullablePath(string? path)
    {
        return string.IsNullOrWhiteSpace(path) ? null : path;
    }
}
