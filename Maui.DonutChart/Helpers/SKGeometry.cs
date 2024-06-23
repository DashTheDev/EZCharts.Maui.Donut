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

    // TODO: Figure out better way, as this was removed from SkiaSharp.Extended and path is too complicated for Path.Contains().
    public static SKPath CreateSectorPath(float start, float end, float outerRadius, float innerRadius = 0.0f, float margin = 0.0f, float explodeDistance = 0.0f, SKPathDirection direction = SKPathDirection.Clockwise)
    {
        SKPath path = new();

        // If the sector has no size, then it has no path
        if (start == end)
        {
            return path;
        }

        // The the sector is a full circle, then do that
        if (end - start == 1.0f)
        {
            path.AddCircle(0, 0, outerRadius, direction);
            path.AddCircle(0, 0, innerRadius, direction);
            path.FillType = SKPathFillType.EvenOdd;
            return path;
        }

        // Calculate the angles
        float startAngle = TotalAngle * start - UprightAngle;
        float endAngle = TotalAngle * end - UprightAngle;
        SKPathArcSize arcSize = endAngle - startAngle > PI ? SKPathArcSize.Large : SKPathArcSize.Small;
        float sectorCenterAngle = (endAngle - startAngle) / 2f + startAngle;

        // Move explosion around 90 degrees, since matrix use down as 0
        SKMatrix explosionMatrix = SKMatrix.CreateRotation(sectorCenterAngle - (PI / 2f));
        SKPoint offset = explosionMatrix.MapPoint(new SKPoint(0, explodeDistance));

        // Calculate the angle for the margins
        margin = direction == SKPathDirection.Clockwise ? margin : -margin;
        float offsetR = outerRadius == 0 ? 0 : (margin / (TotalAngle * outerRadius) * TotalAngle);
        float offsetr = innerRadius == 0 ? 0 : (margin / (TotalAngle * innerRadius) * TotalAngle);

        // Get the points
        SKPoint a = GetCirclePoint(outerRadius, startAngle + offsetR) + offset;
        SKPoint b = GetCirclePoint(outerRadius, endAngle - offsetR) + offset;
        SKPoint c = GetCirclePoint(innerRadius, endAngle - offsetr) + offset;
        SKPoint d = GetCirclePoint(innerRadius, startAngle + offsetr) + offset;

        // Add the points to the path
        path.MoveTo(a);
        path.ArcTo(outerRadius, outerRadius, 0, arcSize, direction, b.X, b.Y);
        path.LineTo(c);

        if (innerRadius == 0.0f)
        {
            // Take a short cut
            path.LineTo(d);
        }
        else
        {
            var reverseDirection = direction == SKPathDirection.Clockwise ? SKPathDirection.CounterClockwise : SKPathDirection.Clockwise;
            path.ArcTo(innerRadius, innerRadius, 0, arcSize, reverseDirection, d.X, d.Y);
        }

        path.Close();
        return path;
    }
}