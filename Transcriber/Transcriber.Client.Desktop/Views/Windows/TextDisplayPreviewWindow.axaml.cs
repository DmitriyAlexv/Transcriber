using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Windows;

namespace Transcriber.Client.Desktop.Views.Windows;

public partial class TextDisplayPreviewWindow: ReactiveWindow<TextDisplayPreviewViewModel>
{
    public TextDisplayPreviewWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.ViewModel)
                .Where(vm => vm != null)
                .Select(vm => vm.WhenAnyValue(v => v.Position))
                .Switch()
                .BindTo(this, x => x.Position)
                .DisposeWith(disposables);
        });
    }
}