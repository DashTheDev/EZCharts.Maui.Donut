[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.NamespacePrefix + nameof(Maui.DonutChart.Controls))]
[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.Namespace)]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(XamlConstants.XamlNamespace, "donut")]

static class XamlConstants
{
    public const string XamlNamespace = "http://schemas.dashthedev.com/maui/donut-chart";
    public const string Namespace = $"Maui.DonutChart";
    public const string NamespacePrefix = $"{Namespace}.";
}