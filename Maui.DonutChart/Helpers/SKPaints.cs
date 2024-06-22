using SkiaSharp.Views.Maui;
using SkiaSharp;

namespace Maui.DonutChart.Helpers;

internal static class SKPaints
{
    internal static SKPaint Fill(Color? color) => new()
    {
        Style = SKPaintStyle.Fill,
        Color = GetSKColor(color)
    };

    internal static SKPaint Stroke(Color? color, float width = 1) => new()
    {
        Style = SKPaintStyle.Stroke,
        StrokeWidth = width,
        Color = GetSKColor(color)
    };

    private static SKColor GetSKColor(Color? color)
        => color?.ToSKColor() ?? SKColor.Empty;
}