using MauiCharts.Donut.Controls;

namespace Microsoft.Maui.Controls;

internal static class BindableObjectExtensions
{
    internal static void Bind(this BindableObject bindableObject, BindableProperty targetProperty, string path, object source)
        => bindableObject.SetBinding(targetProperty, new Binding(path, source: source));

    internal static DonutChartView ToDonutChartView(this BindableObject bindableObject)
        => (DonutChartView)bindableObject;
}