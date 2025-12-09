using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using ReactiveUI;
using Transcriber.Client.Desktop.ViewModels.Windows;

namespace Transcriber.Client.Desktop.Views.Windows;

public partial class TextDisplayWindow : ReactiveWindow<TextDisplayViewModel>
{
    public TextDisplayWindow()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (ViewModel != null)
            {
                ViewModel.WhenAnyValue(x => x.Position)
                    .Subscribe(position =>
                    {
                        Position = new PixelPoint(position.X, position.Y);
                    })
                    .DisposeWith(disposables);
                    
                ViewModel.WhenAnyValue(x => x.IsVisible)
                    .Subscribe(isVisible =>
                    {
                        // Анимация появления/исчезновения
                        var border = this.FindControl<Border>("MainBorder");
                        if (border != null)
                        {
                            var animation = new Avalonia.Animation.Animation
                            {
                                Duration = TimeSpan.FromMilliseconds(300),
                                FillMode = Avalonia.Animation.FillMode.Forward
                            };
                            
                            animation.Children.Add(
                                new Avalonia.Animation.KeyFrame
                                {
                                    Cue = new Avalonia.Animation.Cue(0),
                                    Setters = 
                                    {
                                        new Setter(Border.OpacityProperty, isVisible ? 0 : ViewModel?.WindowOpacity ?? 0.8)
                                    }
                                });
                            
                            animation.Children.Add(
                                new Avalonia.Animation.KeyFrame
                                {
                                    Cue = new Avalonia.Animation.Cue(1),
                                    Setters = 
                                    {
                                        new Setter(Border.OpacityProperty, isVisible ? ViewModel?.WindowOpacity ?? 0.8 : 0)
                                    }
                                });
                            
                        }
                    })
                    .DisposeWith(disposables);
            }
        });
    }
}