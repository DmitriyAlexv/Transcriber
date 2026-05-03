using System;
using System.Reactive.Linq;
using ReactiveUI;
using Transcriber.Client.Desktop.Models.Enums;

namespace Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;

public class BottomNavigationBarViewModel: ViewModelBase
{
    private StartViewModel StartViewModel { get; } = new();
    private SettingsViewModel SettingsViewModel { get; } = new();
    
    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentViewModel;
    private ViewModelType _currentViewModelType = ViewModelType.Home;
    
    public BottomNavigationButtonViewModel HomeButtonViewModel { get; }
    public BottomNavigationButtonViewModel SettingsButtonViewModel { get; }
    
    private ViewModelType CurrentViewModelType
    {
        get => _currentViewModelType;
        set => this.RaiseAndSetIfChanged(ref _currentViewModelType, value);
    }
    
    public ViewModelBase CurrentViewModel => _currentViewModel.Value;
    
    public BottomNavigationBarViewModel()
    {
        var navigateHomeCommand = ReactiveCommand.Create(() => NavigateTo(ViewModelType.Home));
        var navigateSettingsCommand = ReactiveCommand.Create(() => NavigateTo(ViewModelType.Settings));
        
        HomeButtonViewModel = new BottomNavigationButtonViewModel(
            this.WhenAnyValue(x => x.CurrentViewModelType)
                .Select(type => type == ViewModelType.Home),
            "Home",
            navigateHomeCommand
        );

        SettingsButtonViewModel = new BottomNavigationButtonViewModel(
            this.WhenAnyValue(x => x.CurrentViewModelType)
                .Select(type => type == ViewModelType.Settings),
            "Settings",
            navigateSettingsCommand
        );
        
        _currentViewModel = this
            .WhenAnyValue(x => x.CurrentViewModelType)
            .Select(viewModelType => viewModelType switch
            {
                ViewModelType.Home => (ViewModelBase)StartViewModel,
                ViewModelType.Settings => SettingsViewModel,
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToProperty(this, x => x.CurrentViewModel);
    }
    
    private void NavigateTo(ViewModelType viewModelType)
    {
        CurrentViewModelType = viewModelType;
    }
}