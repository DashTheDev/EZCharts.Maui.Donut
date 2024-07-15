namespace EZCharts.Maui.Donut.Models;

internal static class Defaults
{
    internal static readonly Thickness Padding = new(10);
    internal const float ChartRotationDegrees = 90f;
    internal const float ChartOuterRadius = 250f;
    internal const float ChartInnerRadius = 125f;

    internal const LabelStyle MainLabelStyle = LabelStyle.Key;
    internal static readonly Color LabelFontColor = Colors.White;
    internal const bool LabelUseAutoFontColor = false;
    internal const string LabelFontFamily = "Arial";
    internal const float LabelFontSize = 20f;
    internal const float LabelKeySpacing = 10f;
    internal const float LabelKeyColorOffset = 20f;
    internal const float LabelOutsideRadius = 50f;

    internal const string EntryValuePath = "Value";
    internal const string EntryLabelPath = "Label";
    internal const float EntryImageScale = 0.1f;

    internal static readonly Color[] ChartColors =
    [
        Colors.Blue,
        Colors.Green,
        Colors.Yellow,
        Colors.Purple,
        Colors.Orange
    ];
}