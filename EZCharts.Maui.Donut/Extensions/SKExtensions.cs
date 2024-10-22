﻿using EZCharts.Maui.Donut.Utility;

namespace SkiaSharp;

internal static class SKExtensions
{
    internal static void SetColor(this SKPaint paint, Color color)
        => paint.Color = SKPaints.GetSKColor(color);
}