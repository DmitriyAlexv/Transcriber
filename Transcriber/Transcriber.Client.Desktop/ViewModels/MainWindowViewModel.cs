using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;
using Transcriber.Client.Desktop.ViewModels.Windows;
using Transcriber.Client.Desktop.Views.Windows;

namespace Transcriber.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase, IActivatableViewModel
{
    private TextDisplayViewModel _textDisplayViewModel;
    private TextDisplayWindow _textDisplayWindow;
    private TextDisplaySettings _textDisplaySettings;
    private IDisposable _textUpdateSubscription;

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
        
        _textDisplayViewModel = new TextDisplayViewModel(LoadDisplaySettings());
        this.WhenActivated(disposables =>
        {
            ShowTextWindow();

            var ctx = new CancellationTokenSource();
            _textUpdateSubscription = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    _textDisplayViewModel.ShowText($"Hello World! {Guid.NewGuid()}");
                })
                .DisposeWith(disposables);


            Disposable.Create(() =>
                {
                    Console.WriteLine("Disposing");
                    ClosePreviewWindow();
                })
                .DisposeWith(disposables);
        });
    }
    
    private void ShowTextWindow()
    {
        if (_textDisplayWindow == null || !_textDisplayWindow.IsVisible)
        {
            _textDisplayWindow = new TextDisplayWindow
            {
                DataContext = _textDisplayViewModel,
                CanResize = false,
                ShowInTaskbar = false,
                Topmost = true,
                ExtendClientAreaToDecorationsHint = true,
                TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur]
            };

            _textDisplayWindow.Closed += (s, e) => _textDisplayWindow = null;
            _textDisplayWindow.Show();
        }
    }
    
    private void UpdatePreviewSettings()
    {
        _textDisplayViewModel?.UpdateSettings(_textDisplaySettings);
    }

    private void ClosePreviewWindow()
    {
        if (_textDisplayWindow == null) 
            return;
        
        _textDisplayWindow.Close();
        _textDisplayWindow = null;
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