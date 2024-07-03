using Android.Graphics;
using Android.Graphics.Drawables;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace EZCharts.Maui.Donut.Utility;

internal static partial class SKBitmaps
{
    internal static SKBitmap? ConvertToSKBitmap(object platformSpecificImage)
    {
        if (platformSpecificImage is not Drawable drawable)
        {
            return null;
        }

        if (Bitmap.Config.Argb8888 is null)
        {
            return null;
        }

        using Bitmap bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth, drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
        using Canvas canvas = new(bitmap);
        drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
        drawable.Draw(canvas);
        return bitmap.ToSKBitmap();
    }
}