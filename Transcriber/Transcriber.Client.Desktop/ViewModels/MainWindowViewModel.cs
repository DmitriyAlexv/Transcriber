using System;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;

namespace Transcriber.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
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
    }
}