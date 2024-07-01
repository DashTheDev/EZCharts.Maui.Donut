using EZCharts.Maui.Donut.Controls;

namespace EZCharts.Maui.Donut.Models;

/// <summary>
/// An enumeration for adjusting how labels are displayed on the <see cref="DonutChartView"/>.
/// </summary>
public enum LabelStyle
{
    /// <summary>
    /// Labels will be displayed to the side of the chart in
    /// a list format with their corresponding chart colors.
    /// </summary>
    Key,

    /// <summary>
    /// Labels will be displayed along the outer radius of the chart.
    /// </summary>
    Outside
}