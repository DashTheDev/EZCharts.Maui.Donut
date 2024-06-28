using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Maui.DonutChart;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Call this to setup Maui.DonutChart.
    /// </summary>
    public static MauiAppBuilder UseDonutChart(this MauiAppBuilder builder)
    {
        return builder.UseSkiaSharp();
    }
}