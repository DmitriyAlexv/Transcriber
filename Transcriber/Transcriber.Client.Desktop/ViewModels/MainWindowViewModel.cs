using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using Transcriber.Client.Desktop.Services;
using Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;
using Transcriber.Client.Desktop.ViewModels.Windows;
using Transcriber.Client.Desktop.Views.Windows;

namespace Transcriber.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
    public BottomNavigationBarViewModel BottomNavigationBarViewModel { get; } = new();
    public ViewModelBase CurrentViewModel => BottomNavigationBarViewModel.CurrentViewModel;

    public MainWindowViewModel()
    {
        BottomNavigationBarViewModel
            .WhenAnyValue(x => x.CurrentViewModel)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(CurrentViewModel));
            });
        
        var textDisplayViewModel = new TextDisplayViewModel();
        var textDisplayWindow = new TextDisplayWindow
        {
            DataContext = textDisplayViewModel,
            CanResize = false,
            ShowInTaskbar = false,
            Topmost = true,
            ExtendClientAreaToDecorationsHint = true,
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur]
        };
        
        Singleton.AppSettingsManager
            .WhenAnyValue(x => x.TextDisplaySettings)
            .Subscribe(x =>
            {
                textDisplayViewModel.RaisePropertyChanged(nameof(textDisplayViewModel.Settings));
            });
        
        this.WhenActivated(disposables =>
        {
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    if(BottomNavigationBarViewModel.StartViewModel.IsButtonEnabled)
                        textDisplayViewModel.ShowText($"Hello World! {Guid.NewGuid()}");
                })
                .DisposeWith(disposables);


            Disposable.Create( () =>
                {
                    Singleton.AppSettingsManager.SaveAll();
                    textDisplayWindow.Close();
                    textDisplayViewModel.Dispose();
                })
                .DisposeWith(disposables);
        });
    }
}