using ReactiveUI;
using Transcriber.Client.Desktop.Models;

namespace Transcriber.Client.Desktop.ViewModels.Windows;

public class TextDisplayPreviewViewModel: ViewModelBase
{
    private TextDisplaySettings _settings;
    private string _displayText = "Пример текста для preview";

    public TextDisplaySettings Settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    public string DisplayText
    {
        get => _displayText;
        set => this.RaiseAndSetIfChanged(ref _displayText, value);
    }

    public TextDisplayPreviewViewModel(TextDisplaySettings settings)
    {
        _settings = settings;
    }
}