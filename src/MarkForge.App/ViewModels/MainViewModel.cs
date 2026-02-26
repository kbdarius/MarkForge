using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarkForge.App.Services;

namespace MarkForge.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        ISettingsService settingsService = new JsonSettingsService();
        IFileDialogService fileDialogService = new FileDialogService();
        IConversionService conversionService = new PlaceholderConversionService();

        ConvertViewModel = new ConvertViewModel(
            settingsService,
            fileDialogService,
            conversionService);
        SettingsViewModel = new SettingsViewModel(settingsService);
        currentViewModel = ConvertViewModel;
    }

    public ConvertViewModel ConvertViewModel { get; }

    public SettingsViewModel SettingsViewModel { get; }

    [ObservableProperty]
    private object currentViewModel;

    public async Task InitializeAsync()
    {
        await ConvertViewModel.LoadSettingsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private void ShowConvertView()
    {
        CurrentViewModel = ConvertViewModel;
    }

    [RelayCommand]
    private async Task ShowSettingsView()
    {
        CurrentViewModel = SettingsViewModel;
        await SettingsViewModel.RefreshCommand.ExecuteAsync(null);
    }
}
