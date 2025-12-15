using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Transcriber.Core.Models;

namespace Transcriber.Client.Desktop.Services;

public class CaptureModeConverter : IValueConverter
{
    public static readonly CaptureModeConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CaptureMode mode)
        {
            return mode switch
            {
                CaptureMode.All => "Все устройства",
                CaptureMode.WhiteList => "Белый список",
                CaptureMode.BlackList => "Чёрный список",
                _ => value.ToString()
            };
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}