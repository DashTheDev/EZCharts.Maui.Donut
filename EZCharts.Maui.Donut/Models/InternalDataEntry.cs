using SkiaSharp;

namespace EZCharts.Maui.Donut.Models;

internal class InternalDataEntry : DataEntry
{
    internal SKSectorPath? SectorPath { get; set; }
    internal object? OriginalEntry { get; set; }
    internal SKBitmap? Image { get; set; }
}