using Avalonia.ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

namespace Transcriber.Client.Desktop.Views.Controls.SettingsView;

public partial class SettingsNavigationView: ReactiveUserControl<SettingsNavigationViewModel>
{
    public SettingsNavigationView()
    {
        InitializeComponent();
    }
}