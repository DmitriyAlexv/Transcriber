using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;

namespace Transcriber.Client.Desktop.ViewModels.Controls.Shared;

public class ListEditorViewModel : ViewModelBase
{
    public class ListItemViewModel(string value) : ReactiveObject
    {
        private string _value = value;
        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }

    private ObservableCollection<ListItemViewModel> _items;

    public ObservableCollection<ListItemViewModel> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    private ListItemViewModel? _selectedItem;

    public ListItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public bool HasSelectedItem => SelectedItem != null;

    private ObservableCollection<string> _sourceCollection;

    public ObservableCollection<string> SourceCollection
    {
        get => _sourceCollection;
        set
        {
            this.RaiseAndSetIfChanged(ref _sourceCollection, value);
            SyncItemsFromSource();
        }
    }

    public ReactiveCommand<Unit, Unit> AddNewItemCommand { get; }
    public ReactiveCommand<Unit, Unit> RemoveSelectedItemCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearAllCommand { get; }

    public ListEditorViewModel(ObservableCollection<string> sourceCollection)
    {
        _sourceCollection = sourceCollection;
        _items = new ObservableCollection<ListItemViewModel>();

        SyncItemsFromSource();

        AddNewItemCommand = ReactiveCommand.Create(() =>
        {
            var newItem = new ListItemViewModel("");
            Items.Add(newItem);
            SyncToSourceCollection();

            SelectedItem = newItem;
        });

        RemoveSelectedItemCommand = ReactiveCommand.Create(() =>
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);
                SyncToSourceCollection();
                SelectedItem = null;
            }
        });

        ClearAllCommand = ReactiveCommand.Create(() =>
        {
            Items.Clear();
            SyncToSourceCollection();
            SelectedItem = null;
        });

        _items.CollectionChanged += (_, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                SyncToSourceCollection();
            }
        };

        this.WhenAnyValue(x => x.Items)
            .Subscribe(items =>
            {
                if (items == null)
                    return;
                foreach (var item in items)
                {
                    item.PropertyChanged -= OnItemValueChanged;
                    item.PropertyChanged += OnItemValueChanged;
                }
            });

        this.WhenAnyValue(x => x.SelectedItem)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(HasSelectedItem)));
    }

    private void OnItemValueChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ListItemViewModel.Value))
        {
            SyncToSourceCollection();
        }
    }

    private void SyncItemsFromSource()
    {
        if (SourceCollection == null) return;

        Items.Clear();
        foreach (var item in SourceCollection)
        {
            Items.Add(new ListItemViewModel(item));
        }
    }

    private void SyncToSourceCollection()
    {
        if (SourceCollection == null) return;

        SourceCollection.Clear();
        foreach (var item in Items.Where(i => !string.IsNullOrWhiteSpace(i.Value)))
        {
            SourceCollection.Add(item.Value.Trim());
        }
    }
}