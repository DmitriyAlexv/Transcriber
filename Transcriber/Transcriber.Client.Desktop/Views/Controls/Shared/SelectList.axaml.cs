using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Controls.Shared;

namespace Transcriber.Client.Desktop.Views.Controls.Shared;

public partial class SelectList : ReactiveUserControl<SelectListViewModel>
{
    public SelectList()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel,
                    vm => vm.AddCommand,
                    view => view.AddButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel,
                    vm => vm.RemoveCommand,
                    view => view.RemoveButton)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.NewItemText,
                    view => view.NewItemTextBox.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel,
                    vm => vm.Items,
                    view => view.ItemsListBox.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(ViewModel,
                    vm => vm.SelectedItem,
                    view => view.ItemsListBox.SelectedItem)
                .DisposeWith(disposables);
        });
    }
    private TextBox NewItemTextBox => this.FindControl<TextBox>("NewItemTextBox");
    private ListBox ItemsListBox => this.FindControl<ListBox>("ItemsListBox");
}