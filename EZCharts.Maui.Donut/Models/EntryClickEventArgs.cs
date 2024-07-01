using EZCharts.Maui.Donut.Controls;

namespace EZCharts.Maui.Donut.Models;

/// <summary>
/// Contains the event arguments associated with an entry clicked event 
/// on the <see cref="DonutChartView"/>.
/// </summary>
public sealed class EntryClickEventArgs(object entry) : EventArgs
{
    /// <summary>
    /// The data entry <see langword="object"/> that was clicked.<br/><br/>
    /// The type of this <see langword="object"/> will be the same as the entry types
    /// bound to the <see cref="DonutChartView.EntriesSource"/>.
    /// </summary>
    public object Entry { get; } = entry;
}