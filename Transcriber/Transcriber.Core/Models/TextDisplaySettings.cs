namespace Transcriber.Core.Models;

public class TextDisplaySettings
{
    public double WindowLeft { get; set; } = 100;
    public double WindowTop { get; set; } = 100;
    public double WindowWidth { get; set; } = 400;
    public double WindowHeight { get; set; } = 200;
    public double WindowOpacity { get; set; } = 0.8;
    public double TextOpacity { get; set; } = 1.0;
    public int FontSize { get; set; } = 14;
}