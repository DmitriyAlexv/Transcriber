using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Transcriber.Client.Desktop.Models.Enums;
using Transcriber.Client.Desktop.ViewModels.Abstractions;
using Transcriber.Client.Desktop.ViewModels.SettingDetails;

namespace Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

public class SettingsNavigationViewModel: ViewModelBase, IHaveTitle
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly AudioSettingDetailsViewModel _audioSettingDetailsViewModel;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly AudioProcessSettingDetailsViewModel _audioProcessSettingDetailsViewModel;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly TextDisplaySettingDetailsViewModel _textDisplaySettingDetailsViewModel;

    private readonly ObservableAsPropertyHelper<string> _currentTitle;
    private readonly ObservableAsPropertyHelper<bool> _isVisibleNavigateBack;
    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentViewModel;
    private SettingDetailsType _settingDetailsType = SettingDetailsType.None;

    public string Title => "Настройки";
    public SettingDetailsNavigationItemViewModel AudioSettingDetailsNavigationItemViewModel { get; }
    public SettingDetailsNavigationItemViewModel AudioProcessingSettingDetailNavigationItemViewModel { get; }
    public SettingDetailsNavigationItemViewModel TextDisplaySettingDetailsNavigationItemViewModel { get; }
    
    public ReactiveCommand<Unit, Unit> NavigateBackCommand { get; }
    
    private SettingDetailsType SettingDetailsType
    {
        get => _settingDetailsType;
        set => this.RaiseAndSetIfChanged(ref _settingDetailsType, value);
    }

    public string CurrentTitle => _currentTitle.Value;
    public ViewModelBase CurrentViewModel => _currentViewModel.Value;
    public bool IsVisibleNavigateBack => _isVisibleNavigateBack.Value; 
    
    public SettingsNavigationViewModel()
    {
        _audioSettingDetailsViewModel = new AudioSettingDetailsViewModel(this);
        _audioProcessSettingDetailsViewModel = new AudioProcessSettingDetailsViewModel(this);
        _textDisplaySettingDetailsViewModel = new TextDisplaySettingDetailsViewModel(this);
        
        var navigateAudioCommand = ReactiveCommand.Create(() => NavigateTo(SettingDetailsType.Audio));
        var navigateAudioProcessingCommand = ReactiveCommand.Create(() => NavigateTo(SettingDetailsType.AudioProcessing));
        var navigateTextCommand = ReactiveCommand.Create(() => NavigateTo(SettingDetailsType.Text));
        NavigateBackCommand = ReactiveCommand.Create(NavigateBack);
        
        AudioSettingDetailsNavigationItemViewModel = new SettingDetailsNavigationItemViewModel(
            icon: "VolumeSource",
            text: "Источники",
            description: "Настройки источников аудио",
            navigateAudioCommand);

        AudioProcessingSettingDetailNavigationItemViewModel = new SettingDetailsNavigationItemViewModel(
            icon: "Tools",
            text: "Обработка",
            description: "Настройки обработки аудио",
            navigateAudioProcessingCommand);

        TextDisplaySettingDetailsNavigationItemViewModel = new SettingDetailsNavigationItemViewModel(
            icon: "TextBox",
            text: "Текст",
            description: "Настройки отображения текста",
            navigateTextCommand);
        
        _currentViewModel = this
            .WhenAnyValue(x => x.SettingDetailsType)
            .Select(viewModelType => viewModelType switch
            {
                SettingDetailsType.None => (ViewModelBase)this,
                SettingDetailsType.Audio => _audioSettingDetailsViewModel,
                SettingDetailsType.AudioProcessing => _audioProcessSettingDetailsViewModel,
                SettingDetailsType.Text => _textDisplaySettingDetailsViewModel,
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToProperty(this, x => x.CurrentViewModel);
        
        _isVisibleNavigateBack = this
            .WhenAnyValue(x => x.SettingDetailsType)
            .Select(viewModelType => viewModelType switch
            {
                SettingDetailsType.None => false,
                _ => true
            })
            .ToProperty(this, x => x.IsVisibleNavigateBack);
        
        _currentTitle = this
            .WhenAnyValue(x => x.CurrentViewModel)
            .Select(currentViewModel => currentViewModel is IHaveTitle titleViewModel ? titleViewModel.Title : Title)
            .ToProperty(this, x => x.CurrentTitle);
    }

    public void NavigateBack()
    {
        SettingDetailsType = SettingDetailsType.None;
    }
    
    private void NavigateTo(SettingDetailsType settingDetailsType)
    {
        SettingDetailsType = settingDetailsType;
    }
}