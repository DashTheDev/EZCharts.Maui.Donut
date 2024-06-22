using Maui.DonutChart.Helpers;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace Maui.DonutChart.Controls;

[ContentProperty(nameof(Entries))]
public class DonutChartView : SKCanvasView
{
    private SKRect _bounds = SKRect.Empty;

    public DonutChartView()
    {
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

        SKSize padding = new(25, 25);
        SKRect innerRect = new()
        {
            Size = _bounds.Size - padding,
            Location = new(_bounds.Left + (padding.Width / 2), _bounds.Top + (padding.Height / 2))
        };

        int colorIndex = 0;
        int maxColorIndex = ColorConstants.DefaultChartColors.Length - 1;
        float totalValue = Entries.Sum(a => Math.Abs(a.Value));
        float startAngle = -90;

        foreach (DataEntry entry in Entries)
        {
            SKPaint paint = SKPaints.Stroke(ColorConstants.DefaultChartColors[colorIndex], 5);

            float valuePercentage = entry.Value / totalValue;
            float endAngle = startAngle + CircleConstants.Degrees * valuePercentage;

            SKPath path = new();
            path.AddArc(innerRect, startAngle, endAngle - startAngle);
            canvas.DrawPath(path, paint);

            startAngle = endAngle;
            colorIndex++;

            if (colorIndex >= maxColorIndex)
            {
                colorIndex = 0;
            }
        }
    }
}
