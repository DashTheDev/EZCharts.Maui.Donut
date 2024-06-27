using System.Collections;
using System.Collections.ObjectModel;
using Maui.DonutChart.Helpers;
using Maui.DonutChart.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.DonutChart.Controls;

[ContentProperty(nameof(Entries))]
public class DonutChartView : SKCanvasView
{
    #region Fields

    private Func<object, float>? _entryValueAccessor;
    private Type? _entryType;
    private DataEntry[] _internalEntries = [];
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
        //Entries.CollectionChanged -= OnEntriesCollectionChanged;
    }

    #endregion

    #region Events

    public event EventHandler<float>? EntryClicked;

    #endregion

    #region Bindable Properties

    public static readonly BindableProperty EntriesProperty = BindableProperty.Create(
        nameof(Entries),
        typeof(IEnumerable),
        typeof(DonutChartView),
        //propertyChanged: OnEntriesPropertyChanged,
        defaultValueCreator: bindable =>
        {
            ObservableCollection<object> entries = [];
            entries.CollectionChanged += bindable.ToDonutChartView().OnEntriesCollectionChanged;
            return entries;
        });

    public IEnumerable Entries
    {
        get => (IEnumerable)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    public static readonly BindableProperty EntryValuePathProperty = BindableProperty.Create(
        nameof(EntryValuePath),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultEntryValuePath,
        propertyChanged: OnEntryValuePathPropertyChanged);

    public string EntryValuePath
    {
        get => (string)GetValue(EntryValuePathProperty);
        set => SetValue(EntryValuePathProperty, value);
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

    // TODO: Handle observable collection properly now that Entries is now IEnumerable
    //private static void OnEntriesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    //{

    //}

    private void OnEntriesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
    }

    private static void OnEntryValuePathPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DonutChartView control = bindable.ToDonutChartView();
        control._entryValueAccessor = null;
        control.InvalidateSurface();
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

        foreach (DataEntry entry in _internalEntries)
        {
            if (entry.Path is not null && entry.Path.Contains(transformedTouchPoint.X, transformedTouchPoint.Y))
            {
                EntryClicked?.Invoke(this, entry.Value);
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
        _internalEntries = ValidateAndPrepareEntries();

        if (_internalEntries.Length == 0)
        {
            return;
        }

        int colorIndex = 0;
        int maxColorIndex = EntryColors.Length - 1;
        float totalValue = _internalEntries.Sum(a => a.Value);
        float percentageFilled = 0.0f;

        canvas.Translate(_chartBoundsCenter);

        foreach (DataEntry entry in _internalEntries)
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

    private DataEntry[] ValidateAndPrepareEntries()
    {
        object[] entries = Entries.Cast<object>().ToArray();

        if (entries.Length == 0)
        {
            return [];
        }

        _entryType = entries[0].GetType();

        if (entries.Any(a => a.GetType() != _entryType))
        {
            throw new ArgumentException("All entries must be of the same type.");
        }

        // TODO: Handle exceptions
        _entryValueAccessor ??= Expressions.CreatePropertyAccessor<float>(_entryType, EntryValuePath);

        List<DataEntry> dataEntries = [];

        foreach (object entry in Entries)
        {
            try
            {
                dataEntries.Add(new DataEntry()
                {
                    Value = _entryValueAccessor(entry)
                });
            }
            catch (InvalidCastException)
            {
                throw new Exception("Expected value to be a float.");
            }
        }

        return [.. dataEntries];
    }

    #endregion
}
