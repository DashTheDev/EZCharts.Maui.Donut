using SkiaSharp;

namespace Maui.DonutChart.Helpers;

// Original version: https://github.com/mono/SkiaSharp/blob/322baee72a018a889e85fc48b42cde9764797dae/source/SkiaSharp.Extended/SkiaSharp.Extended.Shared/SKGeometry.cs#L19-L79
internal static class SKGeometry
{
    internal static SKPath CreateSectorPath(float startPercentage, float endPercentage, float outerRadius, float innerRadius, float rotationDegrees)
    {
        float startAngle = GetDegrees(startPercentage, rotationDegrees);
        float endAngle = GetDegrees(endPercentage, rotationDegrees);
        float sweepAngle = endAngle - startAngle;

        SKRect outerRect = new(-outerRadius, -outerRadius, outerRadius, outerRadius);
        SKRect innerRect = new(-innerRadius, -innerRadius, innerRadius, innerRadius);
        SKPoint outerStartPoint = GetCirclePoint(outerRadius, GetRadians(startAngle));
        SKPoint innerEndPoint = GetCirclePoint(innerRadius, GetRadians(endAngle));

        SKPath path = new();
        path.MoveTo(outerStartPoint);
        path.ArcTo(outerRect, startAngle, sweepAngle, false);
        path.LineTo(innerEndPoint);
        path.ArcTo(innerRect, endAngle, -sweepAngle, false);
        path.Close();
        return path;
    }

    internal static SKPoint GetCirclePoint(float radius, float angle)
        => new(radius * MathF.Cos(angle), radius * MathF.Sin(angle));

    internal static SKPoint ReverseTransformations(SKPoint point, SKPoint translation, float scale = 1)
        => new((point.X - translation.X) / scale, (point.Y - translation.Y) / scale);

    internal static float GetRadians(float degrees)
        => degrees * MathF.PI / 180;

    internal static float GetDegrees(float percentage, float rotationDegrees) 
        => percentage * 360 - rotationDegrees;
}