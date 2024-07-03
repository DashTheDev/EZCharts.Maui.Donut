using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using EZCharts.Maui.Donut.Utility;
using EZCharts.Maui.Donut.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace EZCharts.Maui.Donut.Controls;

/// <summary>
/// A highly customisable <see cref="SKCanvasView"/> tailored to displaying
/// data in a donut segmented chart.
/// </summary>
[ContentProperty(nameof(EntriesSource))]
public class DonutChartView : SKCanvasView, IPadding
{
    #region Fields

    private Func<object, float>? _entryValueAccessor;
    private Func<object, string>? _entryLabelAccessor;
    private INotifyCollectionChanged? _observableEntries;
    private InternalDataEntry[] _internalEntries = [];
    private SKRect _canvasBounds = SKRect.Empty;
    private SKRect _chartBounds = SKRect.Empty;
    private SKRect? _textBounds = SKRect.Empty;

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
    public event EventHandler<EntryClickEventArgs>? EntryClicked;

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

    /// <summary>Bindable property for <see cref="EntryIconTemplate"/>.</summary>
    public static readonly BindableProperty EntryIconTemplateProperty = BindableProperty.Create(
        nameof(EntryIconTemplate),
        typeof(DataTemplate),
        typeof(DonutChartView));

    /// <summary>
    /// Gets or sets the template to be used for rendering the icon for each entry.<br/>
    /// It is expected that the templated view is/or is derived from an <see cref="ImageSource"/>.<br/><br/>
    /// This is a bindable property which defaults to <b><see langword="null"/></b>.
    /// </summary>
    public DataTemplate? EntryIconTemplate
    {
        get => (DataTemplate?)GetValue(EntryIconTemplateProperty);
        set => SetValue(EntryIconTemplateProperty, value);
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

    /// <summary>Bindable property for <see cref="EntryClickedCommand"/>.</summary>
    public static readonly BindableProperty EntryClickedCommandProperty = BindableProperty.Create(
        nameof(EntryClickedCommand),
        typeof(ICommand),
        typeof(DonutChartView));

    /// <summary>
    /// Gets or sets the command to be invoked when an entry is clicked.<br/>
    /// The command will receive an <see langword="object"/> parameter which represents the clicked entry.<br/><br/>
    /// This is a bindable property which defaults to <b><see langword="null"/></b>.
    /// </summary>
    public ICommand? EntryClickedCommand
    {
        get => (ICommand?)GetValue(EntryClickedCommandProperty);
        set => SetValue(EntryClickedCommandProperty, value);
    }

    /// <summary>Bindable property for <see cref="Padding"/>.</summary>
    public static readonly BindableProperty PaddingProperty = BindableProperty.Create(
        nameof(Padding),
        typeof(Thickness),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultPadding,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the padding of the entire view.<br/><br/>
    /// This is a bindable property which defaults to <c>10d</c>.
    /// </summary>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
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
    /// Gets or sets how big the outer circle of the chart will be.<br/>
    /// If the provided value is too big for the display, it will be adjusted to fit.<br/><br/>
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
    /// Gets or sets how big the inner circle of the chart will be.<br/>
    /// If the provided value is too big for the display, it will be adjusted to fit.<br/><br/>
    /// This is a bindable property which defaults to <c>125f</c>.
    /// </summary>
    public float ChartInnerRadius
    {
        get => (float)GetValue(ChartInnerRadiusProperty);
        set => SetValue(ChartInnerRadiusProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelStyle"/>.</summary>
    public static readonly BindableProperty LabelStyleProperty = BindableProperty.Create(
        nameof(LabelStyle),
        typeof(LabelStyle),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelStyle,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the style to be used for chart labels.<br/><br/>
    /// This is a bindable property which defaults to <b><see cref="LabelStyle.Key"/></b>.
    /// </summary>
    public LabelStyle LabelStyle
    {
        get => (LabelStyle)GetValue(LabelStyleProperty);
        set => SetValue(LabelStyleProperty, value);
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
    /// Gets or sets the color of the font used for the chart labels.<br/>
    /// This value will be ignored if <c>LabelUseAutoFontColor</c> is set to <b><see langword="true"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>White</c>.
    /// </summary>
    public Color LabelFontColor
    {
        get => (Color)GetValue(LabelFontColorProperty);
        set => SetValue(LabelFontColorProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelUseAutoFontColor"/>.</summary>
    public static readonly BindableProperty LabelUseAutoFontColorProperty = BindableProperty.Create(
        nameof(LabelUseAutoFontColor),
        typeof(bool),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelUseAutoFontColor,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets if the label font colors will be assigned based on their corresponding entry color.<br/><br/>
    /// This is a bindable property which defaults to <b><see langword="false"/></b>.
    /// </summary>
    public bool LabelUseAutoFontColor
    {
        get => (bool)GetValue(LabelUseAutoFontColorProperty);
        set => SetValue(LabelUseAutoFontColorProperty, value);
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

    /// <summary>Bindable property for <see cref="LabelKeySpacing"/>.</summary>
    public static readonly BindableProperty LabelKeySpacingProperty = BindableProperty.Create(
        nameof(LabelKeySpacing),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelKeySpacing,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the spacing between each chart label.<br/>
    /// This value is only applied when <c>LabelStyle</c> is set to <b><see cref="LabelStyle.Key"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>10f</c>.
    /// </summary>
    public float LabelKeySpacing
    {
        get => (float)GetValue(LabelKeySpacingProperty);
        set => SetValue(LabelKeySpacingProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelKeyColorOffset"/>.</summary>
    public static readonly BindableProperty LabelKeyColorOffsetProperty = BindableProperty.Create(
        nameof(LabelKeyColorOffset),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelKeyColorOffset,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the horizontal offset of the color circles rendered next to each label.<br/>
    /// This value is only applied when <c>LabelStyle</c> is set to <b><see cref="LabelStyle.Key"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>20f</c>.
    /// </summary>
    public float LabelKeyColorOffset
    {
        get => (float)GetValue(LabelKeyColorOffsetProperty);
        set => SetValue(LabelKeyColorOffsetProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelOutsideRadius"/>.</summary>
    public static readonly BindableProperty LabelOutsideRadiusProperty = BindableProperty.Create(
        nameof(LabelOutsideRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Constants.DefaultLabelOutsideRadius,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the radius from the <c>ChartOuterRadius</c> where the outside labels will be rendered.<br/>
    /// This value is only applied when <c>LabelStyle</c> is set to <b><see cref="LabelStyle.Outside"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>50f</c>.
    /// </summary>
    public float LabelOutsideRadius
    {
        get => (float)GetValue(LabelOutsideRadiusProperty);
        set => SetValue(LabelOutsideRadiusProperty, value);
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
            if (entry.SectorPath is null || entry.OriginalEntry is null)
            {
                continue;
            }

            if (entry.SectorPath.Contains(e.Location.X, e.Location.Y))
            {
                EntryClicked?.Invoke(this, new EntryClickEventArgs(entry.OriginalEntry));
                EntryClickedCommand?.Execute(entry.OriginalEntry);
            }
        }
    }

    #endregion

    #region Supporting Methods

    private void RenderChart(SKCanvas canvas, int width, int height)
    {
        canvas.Clear();

        _internalEntries = ValidateAndPrepareEntries();
        _canvasBounds = new(0, 0, width, height);
        _chartBounds = CreateChartBounds();
        _textBounds = CreateTextBounds();

        RenderBackground(canvas);
        RenderValues(canvas);
        RenderLabels(canvas);
        RenderIcons(canvas);
    }

    private SKRect CreateChartBounds()
    {
        float width = LabelStyle == LabelStyle.Key ? _canvasBounds.Width * 0.75f : _canvasBounds.Width;
        return SKGeometry.CreatePaddedRect(0, 0, width, _canvasBounds.Height, Padding);
    }

    private SKRect? CreateTextBounds()
    {
        if (LabelStyle == LabelStyle.Outside)
        {
            return null;
        }

        return SKGeometry.CreatePaddedRect(_chartBounds.Width, 0, _canvasBounds.Width, _canvasBounds.Height, Padding);
    }

    private void RenderBackground(SKCanvas canvas)
    {
        canvas.DrawRect(_canvasBounds, SKPaints.Fill(BackgroundColor));
        //canvas.DrawRect(_chartBounds, SKPaints.Fill(Colors.Red.WithAlpha(0.25f)));
        //canvas.DrawRect(_textBounds, SKPaints.Fill(Colors.Green.WithAlpha(0.25f)));
    }

    private void RenderValues(SKCanvas canvas)
    {
        if (_internalEntries.Length == 0)
        {
            return;
        }

        ColorSelector entryColorSelector = new(EntryColors);
        float totalValue = _internalEntries.Sum(a => a.Value);
        float percentageFilled = 0.0f;

        // Fit radiuses to bounds, otherwise chart will go off screen if too big
        float outerRadius = MathF.Min(_chartBounds.Width.Halved(), ChartOuterRadius);
        float innerRadius = MathF.Min(_chartBounds.Width.Halved(), ChartInnerRadius);

        foreach (InternalDataEntry entry in _internalEntries)
        {
            SKPaint paint = SKPaints.Fill(entryColorSelector.Next());

            float percentageToFill = entry.Value / totalValue;
            float targetPercentageFilled = percentageFilled + percentageToFill;

            entry.SectorPath = SKGeometry.CreateSectorPath(_chartBounds.MidX, _chartBounds.MidY, percentageFilled, targetPercentageFilled, outerRadius, innerRadius, ChartRotationDegrees);
            canvas.DrawPath(entry.SectorPath, paint);

            percentageFilled = targetPercentageFilled;
        }
    }

    // TODO: Add support for changing text positioning
    // TODO: Add support for label template replacements
    private void RenderLabels(SKCanvas canvas)
    {
        if (_internalEntries.Length == 0)
        {
            return;
        }

        SKPaint textPaint = SKPaints.Text(LabelFontFamily, LabelFontColor, LabelFontSize);
        ColorSelector entryColorSelector = new(EntryColors);

        switch (LabelStyle)
        {
            case LabelStyle.Key:
                RenderKeyLabels(canvas, textPaint, entryColorSelector);
                break;

            default:
                RenderOutsideLabels(canvas, textPaint, entryColorSelector);
                break;
        };
    }

    private void RenderKeyLabels(SKCanvas canvas, SKPaint textPaint, ColorSelector entryColorSelector)
    {
        // Text bounds should be set if LabelStyle is key, but we need to be safe
        if (_textBounds is null)
        {
            return;
        }

        float totalTextHeight = _internalEntries.Length * textPaint.TextSize + (_internalEntries.Length - 1) * LabelKeySpacing;
        float circleRadius = LabelFontSize.Halved();
        float circleRadiusHalved = circleRadius.Halved();
        float maxWidth = _internalEntries
            .Select(e => textPaint.MeasureText(e.Label))
            .Max();

        float startX = _textBounds.Value.Left + (_textBounds.Value.Width - maxWidth) / 2 + LabelKeyColorOffset;
        float startY = _textBounds.Value.MidY - totalTextHeight / 2;

        for (int i = 0; i < _internalEntries.Length; i++)
        {
            Color entryColor = entryColorSelector.Next();

            if (LabelUseAutoFontColor)
            {
                textPaint.SetColor(entryColor);
            }

            SKPaint circlePaint = SKPaints.Fill(entryColor);
            float y = startY + i * (textPaint.TextSize + LabelKeySpacing);
            canvas.DrawText(_internalEntries[i].Label, startX, y, textPaint);
            canvas.DrawCircle(startX - LabelKeyColorOffset, y - circleRadiusHalved, circleRadius, circlePaint);
        }
    }

    private void RenderOutsideLabels(SKCanvas canvas, SKPaint textPaint, ColorSelector entryColorSelector)
    {
        float labelOutsideRadius = ChartOuterRadius + LabelOutsideRadius;

        foreach (InternalDataEntry entry in _internalEntries.Where(e => e.SectorPath is not null))
        {
            if (LabelUseAutoFontColor)
            {
                Color entryColor = entryColorSelector.Next();
                textPaint.SetColor(entryColor);
            }

            SKPoint sectorMidpoint = SKGeometry.GetSectorMidpoint(entry.SectorPath!, labelOutsideRadius);

            if (_internalEntries.Length == 1)
            {
                textPaint.TextAlign = SKTextAlign.Center;
            }
            else
            {
                bool shouldAlignRight = sectorMidpoint.X - entry.SectorPath!.CenterX < 0;
                textPaint.TextAlign = shouldAlignRight ? SKTextAlign.Right : SKTextAlign.Left;
            }

            canvas.DrawText(entry.Label, sectorMidpoint, textPaint);
        }
    }

    // NOTE: Not a massive fan of this approach with the template and expected ImageSource
    // TODO: Setup FileImageSource and FontImageSource samples
    // TODO: Mega cleanup and optimisation
    // TODO: Scale bindable property
    // TODO: Handle can't load image exceptions
    private void RenderIcons(SKCanvas canvas)
    {
        if (_internalEntries.Length == 0 || EntryIconTemplate is null)
        {
            return;
        }

        if (this.FindMauiContext() is not IMauiContext mauiContext)
        {
            return;
        }

        foreach (InternalDataEntry entry in _internalEntries.Where(e => e.SectorPath is not null))
        {
            if (EntryIconTemplate.CreateContent() is not ImageSource iconImageSource)
            {
                break;
            }

            SKPoint sectorMidpoint = SKGeometry.GetSectorMidpoint(entry.SectorPath!, ChartOuterRadius - ((ChartOuterRadius - ChartInnerRadius) / 2));

            iconImageSource.BindingContext = entry.OriginalEntry;
            iconImageSource.LoadImage(mauiContext, result =>
            {
                if (result is null)
                {
                    return;
                }

                SKBitmap? skBitmap = SKBitmaps.ConvertToSKBitmap(result.Value);

                if (skBitmap is null)
                {
                    return;
                }

                //canvas.DrawCircle(sectorMidpoint, 10, SKPaints.Fill(Colors.Red));

                SKBitmap scaledBitmap = SKBitmaps.Scale(skBitmap, 0.1f);
                sectorMidpoint.X -= scaledBitmap.Width / 2;
                sectorMidpoint.Y -= scaledBitmap.Height / 2;
                canvas.DrawBitmap(scaledBitmap, sectorMidpoint);
            });
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
                    OriginalEntry = entry,
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