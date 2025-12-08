using System;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

namespace Transcriber.Client.Desktop.ViewModels;

public class SettingsViewModel: ViewModelBase
{
    public SettingsNavigationViewModel SettingsNavigationViewModel { get; } = new();
    public ViewModelBase CurrentViewModel => SettingsNavigationViewModel.CurrentViewModel;
        
    public SettingsViewModel()
    {
        SettingsNavigationViewModel
            .WhenAnyValue(x => x.CurrentViewModel)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(CurrentViewModel));
            });
    }
}