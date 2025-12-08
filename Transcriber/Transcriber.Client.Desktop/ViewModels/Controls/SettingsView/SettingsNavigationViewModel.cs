using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Transcriber.Client.Desktop.Models.Enums;
using Transcriber.Client.Desktop.ViewModels.SettingDetails;

namespace Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

public class SettingsNavigationViewModel: ViewModelBase
{
    private readonly AudioSettingDetailsViewModel _audioSettingDetailsViewModel;
    private readonly AudioProcessSettingDetailsViewModel _audioProcessSettingDetailsViewModel;
    private readonly TextDisplaySettingDetailsViewModel _textDisplaySettingDetailsViewModel;

    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentViewModel;
    private SettingDetailsType _settingDetailsType = SettingDetailsType.None;
    
    public SettingDetailsNavigationItemViewModel AudioSettingDetailsNavigationItemViewModel { get; }
    public SettingDetailsNavigationItemViewModel AudioProcessingSettingDetailNavigationItemViewModel { get; }
    public SettingDetailsNavigationItemViewModel TextDisplaySettingDetailsNavigationItemViewModel { get; }
    
    private SettingDetailsType SettingDetailsType
    {
        get => _settingDetailsType;
        set => this.RaiseAndSetIfChanged(ref _settingDetailsType, value);
    }
    
    public ViewModelBase CurrentViewModel => _currentViewModel.Value;
    
    public SettingsNavigationViewModel()
    {
        _audioSettingDetailsViewModel = new AudioSettingDetailsViewModel(this);
        _audioProcessSettingDetailsViewModel = new AudioProcessSettingDetailsViewModel(this);
        _textDisplaySettingDetailsViewModel = new TextDisplaySettingDetailsViewModel(this);
        
        var navigateAudioCommand = ReactiveCommand.Create(() => NavigateTo(SettingDetailsType.Audio));
        var navigateAudioProcessingCommand = ReactiveCommand.Create(() => NavigateTo(SettingDetailsType.AudioProcessing));
        var navigateTextCommand = ReactiveCommand.Create(() => NavigateTo(SettingDetailsType.Text));
        
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