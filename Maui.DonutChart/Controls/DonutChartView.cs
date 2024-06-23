using Maui.DonutChart.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.DonutChart.Controls;

[ContentProperty(nameof(Entries))]
public class DonutChartView : SKCanvasView
{
    private SKRect _bounds = SKRect.Empty;

    public DonutChartView()
    {
        EnableTouchEvents = true;
        PaintSurface += OnPaintSurface;
    }

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

    private void OnEntriesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
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

        foreach (DataEntry entry in Entries)
        {
            if (entry.Path is not null && entry.Path.Contains(e.Location.X, e.Location.Y))
            {
                entry.InvokeClicked();
            }
        }
    }

    private void RenderChart(SKCanvas canvas, int width, int height)
    {
        _bounds = new SKRect(0, 0, width, height);
        RenderBackground(canvas);
        RenderData(canvas);
    }

    private void RenderBackground(SKCanvas canvas)
    {
        canvas.DrawRect(_bounds, SKPaints.Fill(BackgroundColor));
    }

    private void RenderData(SKCanvas canvas)
    {
        if (Entries.Count == 0)
        {
            return;
        }

        int colorIndex = 0;
        int maxColorIndex = ColorConstants.DefaultChartColors.Length - 1;
        float totalValue = Entries.Sum(a => a.Value);
        float percentageFilled = 0.0f;

        canvas.Translate(_bounds.Width / 2, _bounds.Height / 2);

        foreach (DataEntry entry in Entries)
        {
            SKPaint paint = SKPaints.Fill(ColorConstants.DefaultChartColors[colorIndex]);

            float percentageToFill = entry.Value / totalValue;
            float targetPercentageFilled = percentageFilled + percentageToFill;

            entry.Path = SKGeometry.CreateSectorPath(_bounds, percentageFilled, targetPercentageFilled);
            canvas.DrawPath(entry.Path, paint);

            percentageFilled = targetPercentageFilled;
            colorIndex++;

            if (colorIndex >= maxColorIndex)
            {
                colorIndex = 0;
            }
        }
    }
}
