using Maui.DonutChart.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.DonutChart.Controls;

[ContentProperty(nameof(Entries))]
public class DonutChartView : SKCanvasView
{
    #region Fields

    private SKRect _chartBounds = SKRect.Empty;
    private SKPoint _chartBoundsCenter = SKPoint.Empty;

    #endregion

    #region Constructor & Destructor

    public DonutChartView()
    {
        EnableTouchEvents = true;
        PaintSurface += OnPaintSurface;
    }

    ~DonutChartView()
    {
        PaintSurface -= OnPaintSurface;
        Entries.CollectionChanged -= OnEntriesCollectionChanged;
    }

    #endregion

    #region Bindable Properties

    public static readonly BindableProperty EntriesProperty = BindableProperty.Create(
        nameof(Entries),
        typeof(DataEntryCollection),
        typeof(DonutChartView),
        defaultValueCreator: bindable =>
        {
            DataEntryCollection entries = [];
            entries.CollectionChanged += bindable.ToDonutChartView().OnEntriesCollectionChanged;
            return entries;
        });

    public DataEntryCollection Entries
    {
        get => (DataEntryCollection)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    public static readonly BindableProperty EntryColorsProperty = BindableProperty.Create(
        nameof(EntryColors),
        typeof(Color[]),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultChartColors,
        propertyChanged: OnVisualPropertyChanged);

    public Color[] EntryColors
    {
        get => (Color[])GetValue(EntryColorsProperty);
        set => SetValue(EntryColorsProperty, value);
    }

    public static readonly BindableProperty ChartRotationDegreesProperty = BindableProperty.Create(
        nameof(ChartRotationDegrees),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: 90f,
        propertyChanged: OnVisualPropertyChanged);

    public float ChartRotationDegrees
    {
        get => (float)GetValue(ChartRotationDegreesProperty);
        set => SetValue(ChartRotationDegreesProperty, value);
    }

    public static readonly BindableProperty ChartOuterRadiusProperty = BindableProperty.Create(
        nameof(ChartOuterRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: 250f,
        propertyChanged: OnVisualPropertyChanged);

    public float ChartOuterRadius
    {
        get => (float)GetValue(ChartOuterRadiusProperty);
        set => SetValue(ChartOuterRadiusProperty, value);
    }

    public static readonly BindableProperty ChartInnerRadiusProperty = BindableProperty.Create(
        nameof(ChartInnerRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: 125f,
        propertyChanged: OnVisualPropertyChanged);

    public float ChartInnerRadius
    {
        get => (float)GetValue(ChartInnerRadiusProperty);
        set => SetValue(ChartInnerRadiusProperty, value);
    }

    #endregion

    #region Event Handling

    private void OnEntriesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
    }

    // TODO: Obviously not ideal to render every time visual property is updated. Probably add support for batch property changes
    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        bindable.ToDonutChartView().InvalidateSurface();
    }

    private void OnPaintSurface(object? sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        RenderChart(e.Surface.Canvas, e.Info.Width, e.Info.Height);
    }

    protected override void OnTouch(SKTouchEventArgs e)
    {
        base.OnTouch(e);

        if (e.ActionType != SKTouchAction.Pressed)
        {
            return;
        }

        SKPoint transformedTouchPoint = SKGeometry.ReverseTransformations(e.Location, _chartBoundsCenter);

        foreach (DataEntry entry in Entries)
        {
            if (entry.Path is not null && entry.Path.Contains(transformedTouchPoint.X, transformedTouchPoint.Y))
            {
                entry.InvokeClicked();
            }
        }
    }

    #endregion

    #region Supporting Methods

    private void RenderChart(SKCanvas canvas, int width, int height)
    {
        SetChartBounds(new SKRect(0, 0, width, height));
        canvas.Clear();
        RenderBackground(canvas);
        RenderData(canvas);
    }

    private void RenderBackground(SKCanvas canvas)
    {
        canvas.DrawRect(_chartBounds, SKPaints.Fill(BackgroundColor));
    }

    private void RenderData(SKCanvas canvas)
    {
        if (Entries.Count == 0)
        {
            return;
        }

        int colorIndex = 0;
        int maxColorIndex = EntryColors.Length - 1;
        float totalValue = Entries.Sum(a => a.Value);
        float percentageFilled = 0.0f;

        canvas.Translate(_chartBoundsCenter);

        foreach (DataEntry entry in Entries)
        {
            SKPaint paint = SKPaints.Fill(EntryColors[colorIndex]);

            float percentageToFill = entry.Value / totalValue;
            float targetPercentageFilled = percentageFilled + percentageToFill;

            entry.Path = SKGeometry.CreateSectorPath(percentageFilled, targetPercentageFilled, ChartOuterRadius, ChartInnerRadius, ChartRotationDegrees);
            canvas.DrawPath(entry.Path, paint);

            percentageFilled = targetPercentageFilled;
            colorIndex++;

            if (colorIndex >= maxColorIndex)
            {
                colorIndex = 0;
            }
        }
    }

    private void SetChartBounds(SKRect value)
    {
        _chartBounds = value;
        _chartBoundsCenter = new(_chartBounds.Width.Halved(), _chartBounds.Height.Halved());
    }

    #endregion
}
