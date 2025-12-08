using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Transcriber.Client.Desktop.ViewModels;
using Transcriber.Client.Desktop.Views;

namespace Transcriber.Client.Desktop;

public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;
    
    public override void Initialize()
    {
        /*
        var services = new ServiceCollection();
        ConfigureServices(services); 
        
        _serviceProvider = services.BuildServiceProvider();
        
        // Установка Locator
        Locator.SetLocator(_serviceProvider.GetRequiredService<IDependencyResolver>());
        */
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Locator.CurrentMutable.Register(() => new StartView(), typeof(IViewFor<StartViewModel>));
        Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    private void ConfigureServices(IServiceCollection services)
    {
        // Регистрация сервисов
        
        // Регистрация ViewModels
        services.AddTransient<MainWindowViewModel>();
        
        // Регистрация Views
        services.AddTransient<MainWindow>();
        
        // Регистрация Locator
        services.AddSingleton<IReadonlyDependencyResolver>(sp => 
            sp.GetRequiredService<IDependencyResolver>());
    }
}