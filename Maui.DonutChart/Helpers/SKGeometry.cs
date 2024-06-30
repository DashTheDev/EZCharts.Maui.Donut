﻿using SkiaSharp;

namespace Maui.DonutChart.Helpers;

// Original version: https://github.com/mono/SkiaSharp/blob/322baee72a018a889e85fc48b42cde9764797dae/source/SkiaSharp.Extended/SkiaSharp.Extended.Shared/SKGeometry.cs#L19-L79
internal static class SKGeometry
{
    internal static SKPath CreateSectorPath(
        float centerX,
        float centerY,
        float startPercentage,
        float endPercentage,
        float outerRadius,
        float innerRadius,
        float rotationDegrees)
    {
        bool isFullCircle = endPercentage - startPercentage == 1;
        float startAngle = GetDegrees(startPercentage, rotationDegrees);
        float endAngle = GetDegrees(endPercentage, rotationDegrees);
        float sweepAngle = endAngle - startAngle;

        SKRect outerRect = GetRadiusRect(centerX, centerY, outerRadius);
        SKRect innerRect = GetRadiusRect(centerX, centerY, innerRadius);
        SKPoint outerStartPoint = GetCirclePoint(centerX, centerY, outerRadius, GetRadians(startAngle));
        SKPoint innerEndPoint = GetCirclePoint(centerX, centerY, innerRadius, GetRadians(endAngle));

        SKPath path = new();
        path.MoveTo(outerStartPoint);

        if (isFullCircle)
        {
            // NOTE: To get SkiaSharp to draw a full circle with Arcs, we have to break down into two half arcs.
            float middleAngle = GetDegrees(0.5f, rotationDegrees);
            float halvedSweepAngle = sweepAngle.Halved();

            path.ArcTo(outerRect, startAngle, halvedSweepAngle, false);
            path.ArcTo(outerRect, middleAngle, halvedSweepAngle, false);
            path.LineTo(innerEndPoint);
            path.ArcTo(innerRect, endAngle, -halvedSweepAngle, false);
            path.ArcTo(innerRect, middleAngle, -halvedSweepAngle, false);
        }
        else
        {
            path.ArcTo(outerRect, startAngle, sweepAngle, false);
            path.LineTo(innerEndPoint);
            path.ArcTo(innerRect, endAngle, -sweepAngle, false);
        }

        path.Close();
        return path;
    }

    private static float GetDegrees(float percentage, float rotationDegrees)
        => percentage * 360 - rotationDegrees;

    private static float GetRadians(float degrees)
        => degrees * MathF.PI / 180;

    private static SKRect GetRadiusRect(float centerX, float centerY, float radius)
        => new(centerX - radius, 
            centerY - radius,
            centerX + radius, 
            centerY + radius);

    private static SKPoint GetCirclePoint(float centerX, float centerY, float radius, float angleRadians)
        => new(centerX + radius * MathF.Cos(angleRadians),
            centerY + radius * MathF.Sin(angleRadians));
}