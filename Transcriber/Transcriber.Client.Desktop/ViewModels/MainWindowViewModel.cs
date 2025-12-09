using System;
using System.Threading.Tasks;
using ReactiveUI;
using Transcriber.Client.Desktop.Models;
using Transcriber.Client.Desktop.ViewModels.Controls.BottomNavigationBar;
using Transcriber.Client.Desktop.ViewModels.Windows;
using Transcriber.Client.Desktop.Views.Windows;

namespace Transcriber.Client.Desktop.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private TextDisplayViewModel _textDisplayVm;
    private TextDisplayWindow _textDisplayWindow;
    private TextDisplaySettings _displaySettings;
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
        
        _displaySettings = LoadDisplaySettings();
        
        // Инициализация окна отображения текста
        InitializeTextDisplayWindow();
        UpdateDisplayedText("Hello World");
    }
    
    private void InitializeTextDisplayWindow()
    {
        _textDisplayVm = new TextDisplayViewModel(_displaySettings);
        _textDisplayWindow = new TextDisplayWindow
        {
            DataContext = _textDisplayVm
        };
        _textDisplayWindow.Show();
    }
    
    // Метод для обновления отображаемого текста (вызывается извне)
    public void UpdateDisplayedText(string text)
    {
        _textDisplayVm?.ShowText(text);
    }
    
    // Метод для очистки текста
    public void ClearDisplayedText()
    {
        _textDisplayVm?.ClearText();
    }
    
    // Метод для обновления настроек
    public void UpdateDisplaySettings(TextDisplaySettings settings)
    {
        _displaySettings = settings;
        _textDisplayVm?.UpdateSettings(settings);
    }
    
    // Метод для активации/деактивации окна
    public void SetTextDisplayActive(bool isActive)
    {
        _textDisplayVm?.SetWindowActive(isActive);
    }
    
    private TextDisplaySettings LoadDisplaySettings()
    {
        // Здесь загрузите настройки из файла конфигурации
        return new TextDisplaySettings
        {
            WindowWidth = 400,
            WindowHeight = 200,
            WindowOpacity = 0.8,
            TextOpacity = 1.0,
            FontSize = 14,
            WindowLeft = 400,
            WindowTop = 400
        };
    }
}