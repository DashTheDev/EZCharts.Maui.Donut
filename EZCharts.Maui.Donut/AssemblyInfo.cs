[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.NamespacePrefix + nameof(EZCharts.Maui.Donut.Controls))]
[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.NamespacePrefix + nameof(EZCharts.Maui.Donut.Models))]
[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.Namespace)]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(XamlConstants.XamlNamespace, "donut")]

static class XamlConstants
{
    public const string XamlNamespace = "http://schemas.dashthedev.com/ez-charts/maui/donut";
    public const string Namespace = $"EZCharts.Maui.Donut";
    public const string NamespacePrefix = $"{Namespace}.";
}