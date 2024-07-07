# [⮪](README.md) DonutChartView
A [SKCanvasView](https://github.com/mono/SkiaSharp/blob/main/source/SkiaSharp.Views.Maui/SkiaSharp.Views.Maui.Controls/SKCanvasView.cs) used to render customisable donut charts.

```C#
public class DonutChartView : SKCanvasView, IPadding
```

**Inheritance:** [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) -> [BindableObject](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.bindableobject?view=net-maui-8.0) -> [Element](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.element?view=net-maui-8.0) -> [NavigableElement](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.navigableelement?view=net-maui-8.0) -> [VisualElement](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.visualelement?view=net-maui-8.0) -> [View](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.view?view=net-maui-8.0) -> [SKCanvasView](https://github.com/mono/SkiaSharp/blob/main/source/SkiaSharp.Views.Maui/SkiaSharp.Views.Maui.Controls/SKCanvasView.cs) -> DonutChartView

**Implements:** [IPadding](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.ipadding?view=net-maui-8.0)

## Events
| Name | Type | Description |
|:-:|:-:|:-:|
| EntryClicked | [`EntryClickEventArgs`](EntryClickEventArgs.md) | Raised whenever a segment on the chart is clicked. |

## Properties
| Name | Type | Bindable | Default | Description |
|:-:|:-:|:-:|:-:|:-:|
| EntriesSource | [`IEnumerable`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable?view=net-8.0) | ✅ | An empty [`ObservableCollection<object>`](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=net-8.0) | The source of entry data to be used for rendering the chart. It is expected that these entries be some sort of class with value and label properties. |
| EntryValuePath | [`string`](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0) | ✅ | `"Value"` | The path of the value property (expected to be of type [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010)) to be used for determining each entry's segment size. |
| EntryLabelPath | [`string`](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0) | ✅ | `"Label"` | The path of the label property (expected to be of type [`string`](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0)) to be used for a friendly display for each entry. |
| EntryIconTemplate | [`DataTemplate?`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.datatemplate?view=net-maui-8.0) | ✅ | `null` | The template to be used for rendering the image for each data entry. This template's resulting view is expected to be a [`FileImageSource`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.fileimagesource?view=net-maui-8.0). |
| EntryImageScale | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `0.1f` | The scale to apply to the rendered entry image. |
| EntryColors | [`Color[]`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.graphics.color?view=net-maui-8.0) | ✅ |  `Blue, Green, Yellow, Purple, Orange` | The colors to use for representing each data entry on the chart. These colors will loop if there are more entries than colors. |
| EntryClickedCommand | [`ICommand?`](https://learn.microsoft.com/en-us/dotnet/api/system.windows.input.icommand?view=net-8.0) | ✅ | `null` | The command to be invoked when an entry is clicked. This command will receive an `object` parameter which represents the clicked entry. |
| Padding | [`Thickness`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.thickness?view=net-maui-8.0) | ✅ | `10, 10, 10, 10` | The padding applied to the entire view. |
| ChartRotationDegrees | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `90f` | The rotation offset to be applied to the chart. |
| ChartOuterRadius | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `250f` | The radius of the outside ring of the chart. |
| ChartInnerRadius | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `125f` | The radius of the inside ring of the chart. |
| LabelStyle | [`LabelStyle`](/LabelStyle.md) | ✅ | `LabelStyle.Key` | The style in which to display labels on the chart. |
| LabelFontFamily | [`string`](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0) | ✅ | `"Arial"` | The system font family to be used for rendering label text. |
| LabelFontColor | [`Color`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.graphics.color?view=net-maui-8.0) | ✅ |  `White` | The color used for rendering label text. |
| LabelUseAutoFontColor | [`bool`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool) | ✅ |  `false` | Determines if each entry's color will be used to render the corresponding label text or not. |
| LabelFontSize | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `20f` | The size used for rendering label text. |
| LabelKeySpacing | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `10f` | The amount of vertical spacing between each label when `LabelStyle` is set to `LabelStyle.Key`. |
| LabelKeyColorOffset | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `20f` | The amount of horizontal spacing between each label and its corresponding color when `LabelStyle` is set to `LabelStyle.Key`. |
| LabelOutsideRadius | [`float`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types#:~:text=.NET%20type-,float,-%C2%B11.5%20x%2010) | ✅ | `50f` | The radius from the `ChartOuterRadius` where entry labels will be rendered when `LabelStyle` is set to `LabelStyle.Outside`. |