using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Windows;

namespace Transcriber.Client.Desktop.Views.Windows;

public partial class TextDisplayWindow : ReactiveWindow<TextDisplayViewModel>
{
    public TextDisplayWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (ViewModel == null) 
                return;
            
            this.WhenAnyValue(x => x.ViewModel)
                .Where(vm => vm != null)
                .Select(vm => vm.WhenAnyValue(v => v.Settings.Position))
                .Switch()
                .BindTo(this, x => x.Position)
                .DisposeWith(disposables);
        });
    }
}