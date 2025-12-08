using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Transcriber.Client.Desktop.ViewModels.Controls.Shared;

public class SelectListViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<ObservableCollection<string>> _items;
    public ObservableCollection<string> Items => _items.Value;

    private string _selectedItem;

    public string SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private string _newItemText = string.Empty;

    public string NewItemText
    {
        get => _newItemText;
        set => this.RaiseAndSetIfChanged(ref _newItemText, value);
    }

    public ReactiveCommand<Unit, Unit> AddCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveCommand { get; }

    public IObservable<bool> CanAdd =>
        this.WhenAnyValue(x => x.NewItemText,
            text => !string.IsNullOrWhiteSpace(text));

    public IObservable<bool> CanRemove =>
        this.WhenAnyValue(x => x.SelectedItem,
            item => !string.IsNullOrEmpty(item));

    public SelectListViewModel()
    {
        var initialItems = new ObservableCollection<string>
        {
            "Элемент 1",
            "Элемент 2",
            "Элемент 3"
        };

        _items = Observable
            .Return(initialItems)
            .ToProperty(this, x => x.Items, initialItems);

        AddCommand = ReactiveCommand.Create(() =>
        {
            if (!string.IsNullOrWhiteSpace(NewItemText))
            {
                Items.Add(NewItemText.Trim());
                NewItemText = string.Empty;
            }
        }, CanAdd);

        RemoveCommand = ReactiveCommand.Create(() =>
        {
            if (!string.IsNullOrEmpty(SelectedItem))
            {
                Items.Remove(SelectedItem);
                SelectedItem = null;
            }
        }, CanRemove);

        this.WhenAnyValue(x => x.Items)
            .Where(items => items.Count == 0)
            .Subscribe(_ => SelectedItem = null);
    }
}