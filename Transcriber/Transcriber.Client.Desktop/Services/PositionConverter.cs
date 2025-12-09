using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Transcriber.Client.Desktop.Services;


public class PositionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PixelPoint pixelPoint)
        {
            // Для отображения в UI (если нужно)
            return $"{pixelPoint.X}, {pixelPoint.Y}";
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}