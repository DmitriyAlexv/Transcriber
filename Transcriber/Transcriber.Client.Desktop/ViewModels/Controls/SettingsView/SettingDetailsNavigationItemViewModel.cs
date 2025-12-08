using System.Reactive;
using ReactiveUI;

namespace Transcriber.Client.Desktop.ViewModels.Controls.SettingsView;

public class SettingDetailsNavigationItemViewModel:  ViewModelBase
{
    public string Icon { get; }
    public string Text { get; }
    public string Description { get; }
    
    public ReactiveCommand<Unit, Unit> NavigateCommand { get; }
    
    public SettingDetailsNavigationItemViewModel(
        string icon, 
        string text, 
        string description,
        ReactiveCommand<Unit, Unit> navigateCommand)
    {
        Icon = icon;
        Text = text;
        Description = description;
        NavigateCommand = navigateCommand;
    }
}