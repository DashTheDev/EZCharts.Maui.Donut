using EZCharts.Maui.Donut.Controls;

namespace EZCharts.Maui.Donut.Models;

/// <summary>
/// A generic entry class for representing data on the <see cref="DonutChartView"/>.
/// </summary>
public class DataEntry
{
    /// <summary>
    /// The <see langword="float"/> value to use for determining 
    /// the percentage of chart area this entry will use.
    /// </summary>
    public float Value { get; set; }

    /// <summary>
    /// The <see langword="string"/> label to use for displaying
    /// what this entry represents.
    /// </summary>
    public string Label { get; set; } = string.Empty;
}