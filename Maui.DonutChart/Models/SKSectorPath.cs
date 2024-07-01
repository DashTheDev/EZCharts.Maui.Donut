using SkiaSharp;

namespace Maui.DonutChart.Models;

internal class SKSectorPath : SKPath
{
    internal float CenterX { get; set; }
    internal float CenterY { get; set; }
    internal float StartPercentage { get; set; }
    internal float EndPercentage { get; set; }
    internal float RotationDegrees { get; set; }
}