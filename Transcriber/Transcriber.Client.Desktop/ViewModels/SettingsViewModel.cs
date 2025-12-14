using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

namespace Transcriber.Client.Desktop.ViewModels;

public class SettingsViewModel: ViewModelBase
{
    public SettingsNavigationViewModel SettingsNavigationViewModel { get; } = new();

    public SettingsViewModel()
    {
    }
}