using SkiaSharp.Views.Maui.Controls.Hosting;

namespace MauiCharts.Donut;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Call this to setup MauiCharts.Donut.
    /// </summary>
    public static MauiAppBuilder UseDonutChart(this MauiAppBuilder builder)
    {
        return builder.UseSkiaSharp();
    }
}