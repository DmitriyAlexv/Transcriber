using System;
using System.Reactive;
using ReactiveUI;

namespace Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;

public class BottomNavigationButtonViewModel: ViewModelBase
{
    private readonly ObservableAsPropertyHelper<bool> _isActive;
    private string _iconKind = string.Empty;
    
    public bool IsActive => _isActive.Value;
    
    public string IconKind
    {
        get => _iconKind;
        set => this.RaiseAndSetIfChanged(ref _iconKind, value);
    }
    
    public ReactiveCommand<Unit, Unit>? Command { get; set; }
    
    public BottomNavigationButtonViewModel(IObservable<bool> isActiveObservable, string iconKind, ReactiveCommand<Unit, Unit> command)
    {
        IconKind = iconKind;
        Command = command;
        _isActive = isActiveObservable
            .ToProperty(this, x => x.IsActive);
    }
}