using Avalonia.ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;

namespace Transcriber.Client.Desktop.Views.Controls.BottomNavigationBar;

public partial class BottomNavigationButton : ReactiveUserControl<BottomNavigationButtonViewModel>
{
    public BottomNavigationButton()
    {
        InitializeComponent();
    }
}