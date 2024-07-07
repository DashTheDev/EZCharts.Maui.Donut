﻿using EZCharts.Maui.Donut.Models;

namespace EZCharts.Maui.Donut;

internal static class Constants
{
    internal const float DefaultEntryImageScale = 0.1f;
    internal static readonly Thickness DefaultPadding = new(10);
    internal const float DefaultChartRotationDegrees = 90f;
    internal const float DefaultChartOuterRadius = 250f;   
    internal const float DefaultChartInnerRadius = 125f;
    internal const LabelStyle DefaultLabelStyle = LabelStyle.Key;
    internal const string DefaultLabelFontFamily = "Arial";
    internal static readonly Color DefaultLabelFontColor = Colors.White;
    internal const bool DefaultLabelUseAutoFontColor = false;
    internal const float DefaultLabelFontSize = 20f;
    internal const float DefaultLabelKeySpacing = 10f;
    internal const float DefaultLabelKeyColorOffset = 20f;
    internal const float DefaultLabelOutsideRadius = 50f;
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