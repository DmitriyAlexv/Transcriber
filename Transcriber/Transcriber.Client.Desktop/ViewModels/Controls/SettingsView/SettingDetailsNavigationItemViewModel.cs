using System.Reactive;
using ReactiveUI;

namespace Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

public class SettingDetailsNavigationItemViewModel(
    string icon,
    string text,
    string description,
    ReactiveCommand<Unit, Unit> navigateCommand)
    : ViewModelBase
{
    public string Icon { get; } = icon;
    public string Text { get; } = text;
    public string Description { get; } = description;

    public ReactiveCommand<Unit, Unit> NavigateCommand { get; } = navigateCommand;
}