using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Transcriber.Client.Desktop.Services;

public class CaptureDeviceConverter: IValueConverter
{
    public static readonly CaptureDeviceConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is KeyValuePair<int, string> mode)
        {
            return mode.Value;
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}