# [⮪](README.md) Setting Entry Images
Once you've [setup](../../README.md#-setting-up) your [`DonutChartView`](../Reference/DonutChartView.md) there's a property called [`EntryImageTemplate`](../Reference/DonutChartView.md) that you can populate to render icons for each entry. This property expects a [`DataTemplate`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.datatemplate?view=net-maui-8.0) that creates a [`FileImageSource`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.fileimagesource?view=net-maui-8.0). 

## Static Image
So if your MAUI project had an image file called `baseball.png`, to render every entry with that image you'd populate the property as follows:

### XAML
```xaml
<donut:DonutChartView.EntryImageTemplate>
    <DataTemplate>
        <FileImageSource File="baseball.png" />
    </DataTemplate>
</donut:DonutChartView.EntryImageTemplate>
```

### Code-Behind
```C#
static object LoadEntryIcon()
{
    return ImageSource.FromFile("baseball.png");
}

MyChartView.EntryImageTemplate = new DataTemplate(LoadEntryIcon);
```

Which might look something like this:

<img src="../../Media/EntryImages-1.png" alt="Example Image #1" width="75%"/>

> [!NOTE]
> If your images are rendering too large or too small, set the [`EntryImageScale`](../Reference/DonutChartView.md) on your [`DonutChartView`](../Reference/DonutChartView.md) accordingly.

## Dynamic Image
In most scenarios you're going to want to set each entry's image depending on a condition. To allow for this the [`EntryImageTemplate`](../Reference/DonutChartView.md) that you set is provided the original entry object as its [binding context](https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/data-binding-basics?view=net-maui-8.0#:~:text=The-,BindingContext,-property%20of%20the), meaning you can handle the image selection however you'd like. However, here's two common approaches.

### Value Converters
In this scenario, we bind to a property called `Category` on the `TestResult` model which is an [`enum`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum). Then we use a [binding value converter](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/converters?view=net-maui-8.0) to convert the `TestResult`'s `Category` to the image file we want to display. Below is the [`IValueConverter`](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/converters?view=net-maui-8.0) we'll use to do the conversion.

```C#
public class ResultCategoryImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ResultCategory category)
        {
            return FasIcons.ScrollFile;
        }

        return category switch
        {
            ResultCategory.English => FasIcons.CommentFile,
            ResultCategory.Mathematics => FasIcons.CalculatorFile,
            ResultCategory.Science => FasIcons.FlaskFile,
            ResultCategory.Geography => FasIcons.BookAtlasFile,
            ResultCategory.Technology => FasIcons.MicrochipFile,
            ResultCategory.Sports => FasIcons.BaseballFile,
            ResultCategory.Music => FasIcons.MusicFile,
            ResultCategory.Drama => FasIcons.MaskFile,
            ResultCategory.Languages => FasIcons.GlobeFile,
            _ => FasIcons.ScrollFile
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

#### XAML
```xaml
<donut:DonutChartView.EntryImageTemplate>
    <DataTemplate x:DataType="m:TestResult">
        <FileImageSource File="{Binding Category, Converter={StaticResource ResultCategoryImageConverter}}" />
    </DataTemplate>
</donut:DonutChartView.EntryImageTemplate>
```

#### Code-Behind
```C#
static object LoadEntryIcon()
{
    FileImageSource imageSource = new();
    Binding categoryBinding = new(
        path: nameof(TestResult.Category),
        converter: new ResultCategoryImageConverter());
    imageSource.SetBinding(FileImageSource.FileProperty, categoryBinding);
    return imageSource;
}

MyChartView.EntryImageTemplate = new DataTemplate(LoadEntryIcon);
```

### Properties
In this scenario, we bind to a [expression body](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/properties#expression-body-definitions) property called `ImagePath` on the `TestResult` model which is a [`string`](https://learn.microsoft.com/en-us/dotnet/api/system.string?view=net-8.0). When you access this property it executes a method that returns the path of the image depending on the `TestResult`'s `Category` property.

#### TestResult.cs
```C#
public class TestResult
{
    public ResultCategory Category { get; set; }
    public string FilePath => GetFilePath();

    private string GetFilePath()
    {
        return Category switch
        {
            ResultCategory.English => FasIcons.CommentFile,
            ResultCategory.Mathematics => FasIcons.CalculatorFile,
            ResultCategory.Science => FasIcons.FlaskFile,
            ResultCategory.Geography => FasIcons.BookAtlasFile,
            ResultCategory.Technology => FasIcons.MicrochipFile,
            ResultCategory.Sports => FasIcons.BaseballFile,
            ResultCategory.Music => FasIcons.MusicFile,
            ResultCategory.Drama => FasIcons.MaskFile,
            ResultCategory.Languages => FasIcons.GlobeFile,
            _ => FasIcons.ScrollFile
        };
    }
}
```

#### XAML
```xaml
<donut:DonutChartView.EntryImageTemplate>
    <DataTemplate x:DataType="m:TestResult">
        <FileImageSource File="{Binding FilePath}" />
    </DataTemplate>
</donut:DonutChartView.EntryImageTemplate>
```

#### Code-Behind
```C#
static object LoadEntryIcon()
{
    FileImageSource imageSource = new();
    Binding filePathBinding = new(path: nameof(TestResult.FilePath));
    imageSource.SetBinding(FileImageSource.FileProperty, filePathBinding);
    return imageSource;
}

MyChartView.EntryImageTemplate = new DataTemplate(LoadEntryIcon);
```

Your images might look something like this:

<img src="../../Media/EntryImages-2.png" alt="Example Image #2" width="75%"/>

> [!NOTE]
> If your images are rendering too large or too small, set the [`EntryImageScale`](../Reference/DonutChartView.md) on your [`DonutChartView`](../Reference/DonutChartView.md) accordingly.