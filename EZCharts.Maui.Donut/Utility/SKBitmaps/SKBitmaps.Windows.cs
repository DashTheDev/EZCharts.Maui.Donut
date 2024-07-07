using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using Windows.Storage.Streams;

namespace EZCharts.Maui.Donut.Utility;

internal static partial class SKBitmaps
{
    internal static SKBitmap? ConvertToSKBitmap(object platformSpecificImage)
    {
        if (platformSpecificImage is not BitmapImage bitmapImage)
        {
            return null;
        }

        RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSource);
        var streamContent = stream.OpenReadAsync().AsTask().Result;
        return SKBitmap.Decode(streamContent.AsStream());
    }
}