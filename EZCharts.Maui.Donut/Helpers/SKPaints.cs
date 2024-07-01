using SkiaSharp.Views.Maui;
using SkiaSharp;

namespace EZCharts.Maui.Donut.Helpers;

internal static class SKPaints
{
    internal static SKPaint Fill(Color? color = null) => new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        Color = GetSKColor(color)
    };

    internal static SKPaint Stroke(Color? color = null, float width = 1) => new()
    {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = width,
        Color = GetSKColor(color)
    };

    internal static SKPaint Text(string? fontFamily = null, Color? color = null, float size = 10) => new()
    {   
        IsAntialias = true,
        TextSize = size,
        Typeface = SKTypeface.FromFamilyName(fontFamily) ?? SKTypeface.Default,
        TextAlign = SKTextAlign.Left,
        Color = GetSKColor(color)
    };

    internal static SKColor GetSKColor(Color? color)
        => color?.ToSKColor() ?? SKColor.Empty;
}