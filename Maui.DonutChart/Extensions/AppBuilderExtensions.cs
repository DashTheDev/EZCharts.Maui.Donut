using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Microsoft.Maui.Hosting;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseDonutChart(this MauiAppBuilder builder)
    {
        return builder.UseSkiaSharp();
    }
}