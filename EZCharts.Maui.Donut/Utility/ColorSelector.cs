namespace EZCharts.Maui.Donut.Utility;

internal class ColorSelector(Color[] colors)
{
    private int index = -1;

    public Color Next()
    {
        index++;
        Color color = colors[index];

        if (index >= colors.Length - 1)
        {
            index = 0;
        }

        return color;
    }
}