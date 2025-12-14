using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Transcriber.Client.Desktop.Services;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.ViewModels;

public class StartViewModel : ViewModelBase
{
    private string _title = "Главная";
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }
    
    private DataCaptureState _currentState = DataCaptureState.Stopped;
    public DataCaptureState CurrentState
    {
        get => _currentState;
        private set
        {
            this.RaiseAndSetIfChanged(ref _currentState, value);
            this.RaisePropertyChanged(nameof(ButtonText));
            this.RaisePropertyChanged(nameof(IsButtonEnabled));
            this.RaisePropertyChanged(nameof(ShowSpinner));
            this.RaisePropertyChanged(nameof(ShowGlowing));
        }
    }
    
    public string ButtonText => CurrentState switch
    {
        DataCaptureState.Stopped => "Старт",
        DataCaptureState.Starting => "Начинаем",
        DataCaptureState.Started => "Стоп",
        DataCaptureState.Stopping => "Останавливаем",
        _ => "Старт"
    };
    
    public bool IsButtonEnabled => CurrentState is DataCaptureState.Stopped or DataCaptureState.Started;
    public bool ShowSpinner => CurrentState is DataCaptureState.Starting or DataCaptureState.Stopping;
    public bool ShowGlowing => CurrentState is DataCaptureState.Starting or DataCaptureState.Started or DataCaptureState.Stopping;
    
    public ReactiveCommand<Unit, Unit> ButtonCommand { get; }
    
    public StartViewModel()
    {
        ButtonCommand = ReactiveCommand.Create(
            ExecuteButtonCommand,
            this.WhenAnyValue(x => x.IsButtonEnabled)
        );
        
        Observable.FromEventPattern<EventHandler<CaptureStatusChangedEventArgs>, CaptureStatusChangedEventArgs>(
                h => Singleton.AudioCaptureService.OnStatusChanged += h,
                h => Singleton.AudioCaptureService.OnStatusChanged -= h)
            .Select(_ => Singleton.AudioCaptureService.State)
            .ObserveOn(RxApp.MainThreadScheduler) 
            .Subscribe(state =>
            {
                CurrentState = state switch
                {
                    DataCaptureState.Started => DataCaptureState.Started,
                    DataCaptureState.Stopped => DataCaptureState.Stopped,
                    _ => CurrentState
                };
            });
    }
    
    private void ExecuteButtonCommand()
    {
        switch (CurrentState)
        {
            case DataCaptureState.Stopped:
                _ = TransitionToStarting();
                break;
            case DataCaptureState.Started:
                _ = TransitionToStopping();
                break;
        }
    }
    
    private async Task TransitionToStarting()
    {
        CurrentState = DataCaptureState.Starting;
        
        Singleton.TranscribedTextCaptureService.StartCapture();
        await Task.Delay(200);
        Singleton.AudioCaptureService.StartCapture();
        
        
    }
    
    private async Task TransitionToStopping()
    {
        CurrentState = DataCaptureState.Stopping;
        
        Singleton.TranscribedTextCaptureService.StopCapture();
        await Task.Delay(200);
        Singleton.AudioCaptureService.StopCapture();
    }
}