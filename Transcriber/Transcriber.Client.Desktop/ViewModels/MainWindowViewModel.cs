using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Controls;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;
using Transcriber.Client.Desktop.ViewModels.Windows;
using Transcriber.Client.Desktop.Views.Windows;

namespace Transcriber.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    private TextDisplaySettings _textDisplaySettings;

    public ViewModelActivator Activator { get; } = new();
    public BottomNavigationBarViewModel BottomNavigationBarViewModel { get; } = new ();
    public ViewModelBase CurrentViewModel => BottomNavigationBarViewModel.CurrentViewModel;

    public MainWindowViewModel()
    {
        BottomNavigationBarViewModel
            .WhenAnyValue(x => x.CurrentViewModel)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(CurrentViewModel));
            });
        
        var textDisplayViewModel = new TextDisplayViewModel(LoadDisplaySettings());
        var textDisplayWindow = new TextDisplayWindow
        {
            DataContext = textDisplayViewModel,
            CanResize = false,
            ShowInTaskbar = false,
            Topmost = true,
            ExtendClientAreaToDecorationsHint = true,
            TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur]
        };
        
        this.WhenActivated(disposables =>
        {
            textDisplayWindow.Show();

            Observable
                .Interval(TimeSpan.FromSeconds(3))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    textDisplayViewModel.ShowText($"Hello World! {Guid.NewGuid()}");
                })
                .DisposeWith(disposables);


            Disposable.Create(() =>
                {
                    textDisplayWindow.Close();
                    textDisplayViewModel.Dispose();
                })
                .DisposeWith(disposables);
        });
    }
    
    private TextDisplaySettings LoadDisplaySettings()
    {
        return new TextDisplaySettings
        {
            WindowWidth = 400,
            WindowHeight = 200,
            WindowOpacity = 0.8,
            TextOpacity = 1.0,
            FontSize = 14,
            WindowLeft = 400,
            WindowTop = 400
        };
    }
}