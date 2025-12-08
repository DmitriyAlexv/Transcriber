using Avalonia.ReactiveUI;
using Transcriber.Client.Desktop.ViewModels;

namespace Transcriber.Client.Desktop.Views;

public partial class SettingsView: ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();
    }
}