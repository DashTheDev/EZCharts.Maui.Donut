using System.Collections.Specialized;
using EZCharts.Maui.Donut.Models;
using EZCharts.Maui.Donut.Utility;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace EZCharts.Maui.Donut.Controls;

/// <summary>
/// A highly customisable <see cref="SKCanvasView"/> tailored to displaying
/// data in a donut segmented chart.
/// </summary>
[ContentProperty(nameof(EntriesSource))]
public partial class DonutChartView : SKCanvasView, IPadding
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

        control.UpdateEntries();
    }

    private void OnEntriesSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateEntries();
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

    private static void OnEntryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        bindable.ToDonutChartView().UpdateEntries();
    }

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

    private void RenderIcons(SKCanvas canvas)
    {
        if (_internalEntries.Length == 0)
        {
            return;
        }

        foreach (InternalDataEntry entry in _internalEntries.Where(e => e.Image is not null))
        {
            float sectorMidRadius = ChartOuterRadius - (ChartOuterRadius - ChartInnerRadius).Halved();
            SKPoint sectorMidpoint = SKGeometry.GetSectorMidpoint(entry.SectorPath!, sectorMidRadius);
            sectorMidpoint.X -= entry.Image!.Width.Halved();
            sectorMidpoint.Y -= entry.Image!.Height.Halved();
            canvas.DrawBitmap(entry.Image, sectorMidpoint);
        }
    }

    // TODO: Need to make sure these processes are async to ensure entries populated
    // before rendering to the canvas.
    // TODO: Need to make sure this is called as little as possible.
    private void UpdateEntries()
    {
        object[] entries = EntriesSource.Cast<object>().ToArray();

        if (entries.Length == 0)
        {
            _internalEntries = [];
            InvalidateSurface();
            return;
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
                InternalDataEntry internalEntry = new()
                {
                    OriginalEntry = entry,
                    Value = _entryValueAccessor(entry),
                    Label = _entryLabelAccessor(entry)
                };

                LoadEntryImage(internalEntry);
                dataEntries.Add(internalEntry);
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

        _internalEntries = [.. dataEntries];
        InvalidateSurface();
    }

    // TODO: Rework entry image system, either get away from ImageSource or allow more ImageSource types
    // TODO: Optimise by caching Bitmaps created
    // TODO: Handle can't load image exceptions
    private void LoadEntryImage(InternalDataEntry entry)
    {
        if (EntryIconTemplate is null)
        {
            return;
        }

        if (this.FindMauiContext() is not IMauiContext mauiContext)
        {
            return;
        }

        if (EntryIconTemplate.CreateContent() is not FileImageSource fileImageSource)
        {
            return;
        }

        fileImageSource.BindingContext = entry.OriginalEntry;
        fileImageSource.LoadImage(mauiContext, result =>
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

            entry.Image = SKBitmaps.Scale(skBitmap, EntryImageScale);
        });
    }

    #endregion
}