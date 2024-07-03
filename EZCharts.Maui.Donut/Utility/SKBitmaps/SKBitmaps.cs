using SkiaSharp;

namespace EZCharts.Maui.Donut.Utility;

internal static partial class SKBitmaps
{
    internal static SKBitmap Scale(SKBitmap bitmap, float scaleAmount)
    {
        int newWidth = (int)Math.Round(bitmap.Width * scaleAmount);
        int newHeight = (int)Math.Round(bitmap.Height * scaleAmount);
        SKBitmap scaledBitmap = new(newWidth, newHeight);

        using SKCanvas canvas = new(scaledBitmap);

        SKRect destRect = new(0, 0, newWidth, newHeight);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(bitmap, destRect);
        return scaledBitmap;
    }
}