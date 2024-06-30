namespace Maui.DonutChart;

internal static class Constants
{
    internal const float DefaultChartRotationDegrees = 90f;   
    internal const float DefaultChartOuterRadius = 250f;   
    internal const float DefaultChartInnerRadius = 125f;
    internal const string DefaultLabelFontFamily = "Arial";
    internal static readonly Color DefaultLabelFontColor = Colors.White;
    internal const float DefaultLabelFontSize = 20f;
    internal const float DefaultLabelSpacing = 10f;
    internal const float DefaultLabelColorOffset = 20f;
    internal const string DefaultEntryValuePath = "Value";   
    internal const string DefaultEntryLabelPath = "Label";   
    internal static readonly Color[] DefaultChartColors =
    [
        Colors.Blue,
        Colors.Green,
        Colors.Yellow,
        Colors.Purple,
        Colors.Orange
    ];
}