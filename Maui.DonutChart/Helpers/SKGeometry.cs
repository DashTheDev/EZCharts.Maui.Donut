using Maui.DonutChart.Controls;
using SkiaSharp;

namespace Maui.DonutChart.Helpers;

// Original version: https://github.com/mono/SkiaSharp/blob/322baee72a018a889e85fc48b42cde9764797dae/source/SkiaSharp.Extended/SkiaSharp.Extended.Shared/SKGeometry.cs#L19-L79
public static class SKGeometry
{
    private const float PI = (float)Math.PI;
    private const float UprightAngle = PI / 2f;
    private const float TotalAngle = 2f * PI;

    public static SKPoint GetCirclePoint(float radius, float angle)
    {
        return new SKPoint(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));
    }

    // TODO: Cleanup and improve. Figure out path is too complicated for Path.Contains().
    public static SKPath CreateSectorPath(SKRect parentRect, float startPercentage, float endPercentage)
    {
        float outerRadius = Math.Max(parentRect.Width, parentRect.Height) / 2;
        float innerRadius = outerRadius / 2;

        float startAngle = CircleConstants.GetDegrees(startPercentage);
        float endAngle = CircleConstants.GetDegrees(endPercentage);
        float sweepAngle = endAngle - startAngle;

        float startAngleRadians = startAngle * (float)Math.PI / 180;
        float endAngleRadians = endAngle * (float)Math.PI / 180;

        var path = new SKPath();

        SKPoint outerStartPoint = GetCirclePoint(outerRadius, startAngleRadians);
        path.MoveTo(outerStartPoint);

        SKRect outerRect = new(-outerRadius, -outerRadius, outerRadius, outerRadius);
        path.ArcTo(outerRect, startAngle, sweepAngle, false);

        SKPoint innerEndPoint = GetCirclePoint(innerRadius, endAngleRadians);
        path.LineTo(innerEndPoint);

        SKRect innerRect = new(-innerRadius, -innerRadius, innerRadius, innerRadius);
        path.ArcTo(innerRect, endAngle, -sweepAngle, false);

        path.Close();
        return path;
    }
}