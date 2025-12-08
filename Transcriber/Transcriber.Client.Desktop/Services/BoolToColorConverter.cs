using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Transcriber.Client.Desktop.Services;

public class BoolToColorConverter : IValueConverter
{
    public static readonly BoolToColorConverter Instance = new();
        
    private static readonly SolidColorBrush ActiveColor = new(Color.Parse("#007AFF"));
    private static readonly SolidColorBrush InactiveColor = new(Color.Parse("#9E9E9E"));
        
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? ActiveColor : InactiveColor;
        }
        return InactiveColor;
    }
        
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}