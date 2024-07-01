[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.NamespacePrefix + nameof(MauiCharts.Donut.Controls))]
[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.NamespacePrefix + nameof(MauiCharts.Donut.Models))]
[assembly: XmlnsDefinition(XamlConstants.XamlNamespace, XamlConstants.Namespace)]

[assembly: Microsoft.Maui.Controls.XmlnsPrefix(XamlConstants.XamlNamespace, "donut")]

static class XamlConstants
{
    public const string XamlNamespace = "http://schemas.dashthedev.com/maui-charts/donut";
    public const string Namespace = $"MauiCharts.Donut";
    public const string NamespacePrefix = $"{Namespace}.";
}