using System.Reactive;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

namespace Transcriber.Client.Desktop.ViewModels.SettingDetails;

public class AudioSettingDetailsViewModel: ViewModelBase
{
    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    
    public AudioSettingDetailsViewModel(SettingsNavigationViewModel settingsNavigationViewModel)
    {
        NavigateBackCommand = ReactiveCommand.Create(settingsNavigationViewModel.NavigateBack);
    }
}