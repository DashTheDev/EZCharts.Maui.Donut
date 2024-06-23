using SkiaSharp;

namespace Maui.DonutChart.Controls;

public class DataEntry : Element
{
    internal SKPath? Path;

    public event EventHandler? Clicked;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(float),
        typeof(DataEntry));

    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty CaptionProperty = BindableProperty.Create(
        nameof(Caption),
        typeof(string),
        typeof(DataEntry));

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    internal void InvokeClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}