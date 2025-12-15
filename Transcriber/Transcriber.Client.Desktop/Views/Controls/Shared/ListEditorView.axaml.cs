using Avalonia.ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.Shared;

namespace Transcriber.Client.Desktop.Views.Controls.Shared;

public partial class ListEditorView : ReactiveUserControl<ListEditorViewModel>
{
    public ListEditorView()
    {
        InitializeComponent();
    }
}