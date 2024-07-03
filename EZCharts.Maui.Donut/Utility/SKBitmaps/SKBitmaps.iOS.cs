using SkiaSharp;

namespace EZCharts.Maui.Donut.Utility;

internal static partial class SKBitmaps
{
    internal static SKBitmap? ConvertToSKBitmap(object platformSpecificImage)
    {
        if (platformSpecificImage is not UIKit.UIImage uiImage)
        {
            return null;
        }

        if (uiImage.AsPNG() is not Foundation.NSData pngImage)
        {
            return null;
        }

        using Stream stream = pngImage.AsStream();
        return SKBitmap.Decode(stream);
    }
}