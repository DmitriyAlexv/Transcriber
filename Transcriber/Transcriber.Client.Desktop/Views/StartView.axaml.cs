using Avalonia.ReactiveUI;
using Transcriber.Client.Desktop.ViewModels;

namespace Transcriber.Client.Desktop.Views;

public partial class StartView : ReactiveUserControl<StartViewModel>
{
    public StartView()
    {
        InitializeComponent();
    }
}