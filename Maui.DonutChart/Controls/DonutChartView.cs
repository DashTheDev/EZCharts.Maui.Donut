using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    private INotifyCollectionChanged? _observableEntries;
    private DataEntry[] _internalEntries = [];
    private SKRect _chartBounds = SKRect.Empty;
    private SKPoint _chartBoundsCenter = SKPoint.Empty;

    #endregion

    #region Constructor & Destructor

    public DonutChartView()
    {
        _observableEntries = Entries as INotifyCollectionChanged;
        EnableTouchEvents = true;
        PaintSurface += OnPaintSurface;
    }

    ~DonutChartView()
    {
        PaintSurface -= OnPaintSurface;
        
        if (_observableEntries is not null)
        {
            _observableEntries.CollectionChanged -= OnEntriesCollectionChanged;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised whenever a segment on the chart is clicked.
    /// </summary>
    public event EventHandler<float>? EntryClicked;

    #endregion

    #region Bindable Properties

    /// <summary>Bindable property for <see cref="Entries"/>.</summary>
    public static readonly BindableProperty EntriesProperty = BindableProperty.Create(
        nameof(Entries),
        typeof(IEnumerable),
        typeof(DonutChartView),
        propertyChanged: OnEntriesPropertyChanged,
        defaultValueCreator: bindable =>
        {
            ObservableCollection<object> entries = [];
            entries.CollectionChanged += bindable.ToDonutChartView().OnEntriesCollectionChanged;
            return entries;
        });

    /// <summary>
    /// Gets or sets the data entries to be used for rendering the chart.<br/><br/>
    /// This is a bindable property which defaults to an empty <see cref="ObservableCollection{T}"/> with type <see cref="object"/>.
    /// </summary>
    public IEnumerable Entries
    {
        get => (IEnumerable)GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryValuePath"/>.</summary>
    public static readonly BindableProperty EntryValuePathProperty = BindableProperty.Create(
        nameof(EntryValuePath),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultEntryValuePath,
        propertyChanged: OnEntryValuePathPropertyChanged);

    /// <summary>
    /// Gets or sets the path of the value property to be accessed on each data entry.<br/><br/>
    /// This is a bindable property which defaults to <c>"Value"</c>.
    /// </summary>
    public string EntryValuePath
    {
        get => (string)GetValue(EntryValuePathProperty);
        set => SetValue(EntryValuePathProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryColors"/>.</summary>
    public static readonly BindableProperty EntryColorsProperty = BindableProperty.Create(
        nameof(EntryColors),
        typeof(Color[]),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultChartColors,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the colors to be used when displaying data.<br/><br/>
    /// This is a bindable property which defaults to <c>Blue,Green,Yellow,Purple,Orange</c>.
    /// </summary>
    public Color[] EntryColors
    {
        get => (Color[])GetValue(EntryColorsProperty);
        set => SetValue(EntryColorsProperty, value);
    }

    /// <summary>Bindable property for <see cref="ChartRotationDegrees"/>.</summary>
    public static readonly BindableProperty ChartRotationDegreesProperty = BindableProperty.Create(
        nameof(ChartRotationDegrees),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: 90f,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the rotation offset of the chart.<br/><br/>
    /// This is a bindable property which defaults to <c>90f</c>.
    /// </summary>
    public float ChartRotationDegrees
    {
        get => (float)GetValue(ChartRotationDegreesProperty);
        set => SetValue(ChartRotationDegreesProperty, value);
    }

    /// <summary>Bindable property for <see cref="ChartOuterRadius"/>.</summary>
    public static readonly BindableProperty ChartOuterRadiusProperty = BindableProperty.Create(
        nameof(ChartOuterRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: 250f,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets how big the outer circle of the chart will be.<br/><br/>
    /// This is a bindable property which defaults to <c>250f</c>.
    /// </summary>
    public float ChartOuterRadius
    {
        get => (float)GetValue(ChartOuterRadiusProperty);
        set => SetValue(ChartOuterRadiusProperty, value);
    }

    /// <summary>Bindable property for <see cref="ChartInnerRadius"/>.</summary>
    public static readonly BindableProperty ChartInnerRadiusProperty = BindableProperty.Create(
        nameof(ChartInnerRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: 125f,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets how big the inner circle of the chart will be.<br/><br/>
    /// This is a bindable property which defaults to <c>125f</c>.
    /// </summary>
    public float ChartInnerRadius
    {
        get => (float)GetValue(ChartInnerRadiusProperty);
        set => SetValue(ChartInnerRadiusProperty, value);
    }

    #endregion

    #region Event Handling

    private static void OnEntriesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DonutChartView control = bindable.ToDonutChartView();

        if (oldValue is INotifyCollectionChanged oldObservableEntries)
        {
            oldObservableEntries.CollectionChanged -= control.OnEntriesCollectionChanged;
            control._observableEntries = null;
        }

        if (newValue is INotifyCollectionChanged newObservableEntries)
        {
            newObservableEntries.CollectionChanged += control.OnEntriesCollectionChanged;
            control._observableEntries = newObservableEntries;
        }

        control.InvalidateSurface();
    }

    private void OnEntriesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
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

    // TODO: Add support for default DataEntry is user doesn't want to use custom
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

        List<DataEntry> dataEntries = [];

        try
        {
            // NOTE: Using compiled expressions rather than reflection to increase performance for accessing properties dynamically
            _entryValueAccessor ??= Expressions.CreatePropertyAccessor<float>(_entryType, EntryValuePath);

            foreach (object entry in Entries)
            {
                dataEntries.Add(new DataEntry()
                {
                    Value = _entryValueAccessor(entry)
                });
            }
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException("Expected value to be a float.");
        }
        catch
        {
            throw new ArgumentException($"Could not find property {EntryValuePath} on Entry type.");
        }

        return [.. dataEntries];
    }

    #endregion
}
