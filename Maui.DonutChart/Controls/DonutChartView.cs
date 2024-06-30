using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Maui.DonutChart.Helpers;
using Maui.DonutChart.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.DonutChart.Controls;

[ContentProperty(nameof(EntriesSource))]
public class DonutChartView : SKCanvasView
{
    #region Fields

    private Func<object, float>? _entryValueAccessor;
    private Func<object, string>? _entryLabelAccessor;
    private INotifyCollectionChanged? _observableEntries;
    private InternalDataEntry[] _internalEntries = [];
    private SKRect _canvasBounds = SKRect.Empty;
    private SKRect _chartBounds = SKRect.Empty;
    private SKRect _textBounds = SKRect.Empty;

    #endregion

    #region Constructor & Destructor

    public DonutChartView()
    {
        _observableEntries = EntriesSource as INotifyCollectionChanged;
        EnableTouchEvents = true;
        PaintSurface += OnPaintSurface;
    }

    ~DonutChartView()
    {
        PaintSurface -= OnPaintSurface;
        
        if (_observableEntries is not null)
        {
            _observableEntries.CollectionChanged -= OnEntriesSourceCollectionChanged;
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

    /// <summary>Bindable property for <see cref="EntriesSource"/>.</summary>
    public static readonly BindableProperty EntriesSourceProperty = BindableProperty.Create(
        nameof(EntriesSource),
        typeof(IEnumerable),
        typeof(DonutChartView),
        propertyChanged: OnEntriesSourcePropertyChanged,
        defaultValueCreator: bindable =>
        {
            ObservableCollection<object> entries = [];
            entries.CollectionChanged += bindable.ToDonutChartView().OnEntriesSourceCollectionChanged;
            return entries;
        });

    /// <summary>
    /// Gets or sets the source of entry data to be used for rendering the chart.<br/><br/>
    /// This is a bindable property which defaults to an empty <see cref="ObservableCollection{T}"/> with type <see cref="object"/>.
    /// </summary>
    public IEnumerable EntriesSource
    {
        get => (IEnumerable)GetValue(EntriesSourceProperty);
        set => SetValue(EntriesSourceProperty, value);
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

    /// <summary>Bindable property for <see cref="EntryLabelPath"/>.</summary>
    public static readonly BindableProperty EntryLabelPathProperty = BindableProperty.Create(
        nameof(EntryLabelPath),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultEntryLabelPath,
        propertyChanged: OnEntryLabelPathPropertyChanged);

    /// <summary>
    /// Gets or sets the path of the label property to be accessed on each data entry.<br/><br/>
    /// This is a bindable property which defaults to <c>"Label"</c>.
    /// </summary>
    public string EntryLabelPath
    {
        get => (string)GetValue(EntryLabelPathProperty);
        set => SetValue(EntryLabelPathProperty, value);
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
        defaultValue: Constants.DefaultChartRotationDegrees,
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
        defaultValue: Constants.DefaultChartOuterRadius,
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
        defaultValue: Constants.DefaultChartInnerRadius,
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

    /// <summary>Bindable property for <see cref="LabelFontColor"/>.</summary>
    public static readonly BindableProperty LabelFontFamilyProperty = BindableProperty.Create(
        nameof(LabelFontFamily),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelFontFamily,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the system font family used for the chart labels.<br/><br/>
    /// This is a bindable property which defaults to <c>"Arial"</c>.
    /// </summary>
    public string LabelFontFamily
    {
        get => (string)GetValue(LabelFontFamilyProperty);
        set => SetValue(LabelFontFamilyProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelFontColor"/>.</summary>
    public static readonly BindableProperty LabelFontColorProperty = BindableProperty.Create(
        nameof(LabelFontColor),
        typeof(Color),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelFontColor,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the color of the font used for the chart labels.<br/><br/>
    /// This is a bindable property which defaults to <c>White</c>.
    /// </summary>
    public Color LabelFontColor
    {
        get => (Color)GetValue(LabelFontColorProperty);
        set => SetValue(LabelFontColorProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelFontSize"/>.</summary>
    public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create(
        nameof(LabelFontSize),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelFontSize,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the size of the font used for the chart labels.<br/><br/>
    /// This is a bindable property which defaults to <c>20f</c>.
    /// </summary>
    public float LabelFontSize
    {
        get => (float)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelSpacing"/>.</summary>
    public static readonly BindableProperty LabelSpacingProperty = BindableProperty.Create(
        nameof(LabelSpacing),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelSpacing,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the spacing between each chart label.<br/><br/>
    /// This is a bindable property which defaults to <c>10f</c>.
    /// </summary>
    public float LabelSpacing
    {
        get => (float)GetValue(LabelSpacingProperty);
        set => SetValue(LabelSpacingProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelColorOffset"/>.</summary>
    public static readonly BindableProperty LabelColorOffsetProperty = BindableProperty.Create(
        nameof(LabelColorOffset),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelColorOffset,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the horizontal offset of the color circles rendered next to each label.<br/><br/>
    /// This is a bindable property which defaults to <c>20f</c>.
    /// </summary>
    public float LabelColorOffset
    {
        get => (float)GetValue(LabelColorOffsetProperty);
        set => SetValue(LabelColorOffsetProperty, value);
    }

    #endregion

    #region Event Handling

    private static void OnEntriesSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DonutChartView control = bindable.ToDonutChartView();

        if (oldValue is INotifyCollectionChanged oldObservableEntries)
        {
            oldObservableEntries.CollectionChanged -= control.OnEntriesSourceCollectionChanged;
            control._observableEntries = null;
        }

        if (newValue is INotifyCollectionChanged newObservableEntries)
        {
            newObservableEntries.CollectionChanged += control.OnEntriesSourceCollectionChanged;
            control._observableEntries = newObservableEntries;
        }

        control.InvalidateSurface();
    }

    private void OnEntriesSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
    }

    private static void OnEntryValuePathPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DonutChartView control = bindable.ToDonutChartView();
        control._entryValueAccessor = null;
        control.InvalidateSurface();
    }

    private static void OnEntryLabelPathPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DonutChartView control = bindable.ToDonutChartView();
        control._entryLabelAccessor = null;
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

        foreach (InternalDataEntry entry in _internalEntries)
        {
            if (entry.Path is not null && entry.Path.Contains(e.Location.X, e.Location.Y))
            {
                EntryClicked?.Invoke(this, entry.Value);
            }
        }
    }

    #endregion

    #region Supporting Methods

    private void RenderChart(SKCanvas canvas, int width, int height)
    {
        canvas.Clear();

        _canvasBounds = new(0, 0, width, height);
        _chartBounds = new(0, 0, width * 0.75f, height);
        _textBounds = new(_chartBounds.Width, 0, width, height);
        _internalEntries = ValidateAndPrepareEntries();

        RenderBackground(canvas);
        RenderValues(canvas);
        RenderLabels(canvas);
    }

    private void RenderBackground(SKCanvas canvas)
    {
        //canvas.DrawRect(_chartBounds, SKPaints.Fill(Colors.Red.WithAlpha(0.25f)));
        //canvas.DrawRect(_textBounds, SKPaints.Fill(Colors.Green.WithAlpha(0.25f)));
        canvas.DrawRect(_canvasBounds, SKPaints.Fill(BackgroundColor));
    }

    private void RenderValues(SKCanvas canvas)
    {
        if (_internalEntries.Length == 0)
        {
            return;
        }

        ColorSelector colorSelector = new(EntryColors);
        float totalValue = _internalEntries.Sum(a => a.Value);
        float percentageFilled = 0.0f;

        foreach (InternalDataEntry entry in _internalEntries)
        {
            SKPaint paint = SKPaints.Fill(colorSelector.Next());

            float percentageToFill = entry.Value / totalValue;
            float targetPercentageFilled = percentageFilled + percentageToFill;

            entry.Path = SKGeometry.CreateSectorPath(_chartBounds.MidX, _chartBounds.MidY, percentageFilled, targetPercentageFilled, ChartOuterRadius, ChartInnerRadius, ChartRotationDegrees);
            canvas.DrawPath(entry.Path, paint);

            percentageFilled = targetPercentageFilled;
        }
    }

    // TODO: Cleanup rendering code
    // TODO: Add support for changing text positioning
    // TODO: True center text
    private void RenderLabels(SKCanvas canvas)
    {
        if (_internalEntries.Length == 0)
        {
            return;
        }

        ColorSelector colorSelector = new(EntryColors);
        SKPaint textPaint = SKPaints.Text(LabelFontFamily, LabelFontColor, LabelFontSize);
        float totalTextHeight = _internalEntries.Length * textPaint.TextSize + (_internalEntries.Length - 1) * LabelSpacing;
        float startY = _textBounds.MidY - totalTextHeight / 2;
        float circleRadius = LabelFontSize.Halved();
        float circleRadiusHalved = circleRadius.Halved();

        for (int i = 0; i < _internalEntries.Length; i++)
        {
            SKPaint circlePaint = SKPaints.Fill(colorSelector.Next());
            float y = startY + i * (textPaint.TextSize + LabelSpacing);
            canvas.DrawText(_internalEntries[i].Label, _textBounds.MidX, y, textPaint);
            canvas.DrawCircle(_textBounds.MidX - LabelColorOffset, y - circleRadiusHalved, circleRadius, circlePaint);
        }
    }

    private InternalDataEntry[] ValidateAndPrepareEntries()
    {
        object[] entries = EntriesSource.Cast<object>().ToArray();

        if (entries.Length == 0)
        {
            return [];
        }

        Type entryType = entries[0].GetType();

        if (entries.Any(a => a.GetType() != entryType))
        {
            throw new ArgumentException("All entries must be of the same type.");
        }

        List<InternalDataEntry> dataEntries = [];

        try
        {
            // NOTE: Using compiled expressions rather than reflection to increase performance for accessing properties dynamically
            _entryValueAccessor ??= Expressions.CreatePropertyAccessor<float>(entryType, EntryValuePath);
            _entryLabelAccessor ??= Expressions.CreatePropertyAccessor<string>(entryType, EntryLabelPath);

            foreach (object entry in EntriesSource)
            {
                dataEntries.Add(new InternalDataEntry()
                {
                    Value = _entryValueAccessor(entry),
                    Label = _entryLabelAccessor(entry)
                });
            }
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"{EntryValuePath} is expected to be a float, and {EntryLabelPath} is expected to be a string.");
        }
        catch
        {
            throw new ArgumentException($"Could not find a property with the name {EntryValuePath} or {EntryLabelPath} on Entry type.");
        }

        return [.. dataEntries];
    }

    #endregion
}
