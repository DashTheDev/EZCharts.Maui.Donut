using SkiaSharp;

namespace Maui.DonutChart.Models;

internal class DataEntry
{
    internal SKPath? Path { get; set; }
    internal float Value { get; set; }
    internal string Label { get; set; } = string.Empty;
}