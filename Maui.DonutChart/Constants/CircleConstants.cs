namespace Maui.DonutChart.Controls;

internal static class CircleConstants
{
    internal const int Degrees = 360;
    internal const int DegreeOffset = 90;

    internal static float GetDegrees(float percentage, bool useOffset = true)
    {
        float degrees = percentage * Degrees;

        if (useOffset)
        {
            degrees -= DegreeOffset;
        }

        return degrees;
    }
}