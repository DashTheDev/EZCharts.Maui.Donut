using SkiaSharp.Views.Maui.Controls.Hosting;

namespace EZCharts.Maui.Donut;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Call this to setup EZCharts.Maui.Donut.
    /// </summary>
    public static MauiAppBuilder UseDonutChart(this MauiAppBuilder builder)
    {
        return builder.UseSkiaSharp();
    }
}