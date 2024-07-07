namespace System;

internal static class SystemExtensions
{
    internal static int Halved(this int value)
        => value / 2;

    internal static float Halved(this float value)
        => value / 2;

    internal static float ToFloat(this double value)
        => (float)value;
}