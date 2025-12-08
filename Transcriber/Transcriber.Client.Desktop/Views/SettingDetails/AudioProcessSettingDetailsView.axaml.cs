using Avalonia.ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.SettingDetails;

namespace Transcriber.Client.Desktop.Views.SettingDetails;

public partial class AudioProcessSettingDetailsView : ReactiveUserControl<AudioProcessSettingDetailsViewModel>
{
    public AudioProcessSettingDetailsView()
    {
        InitializeComponent();
    }
}