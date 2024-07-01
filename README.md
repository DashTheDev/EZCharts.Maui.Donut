# 🍩 EZCharts.Maui.Donut
[![NuGet Version](https://img.shields.io/nuget/v/EZCharts.Maui.Donut)](https://www.nuget.org/packages/EZCharts.Maui.Donut)
[![GitHub License](https://img.shields.io/github/license/DashTheDev/EZCharts.Maui.Donut?style=flat)](https://github.com/DashTheDev/EZCharts.Maui.Donut/blob/master/LICENSE)
[![GitHub Last Commit](https://img.shields.io/github/last-commit/DashTheDev/EZCharts.Maui.Donut)](https://github.com/DashTheDev/EZCharts.Maui.Donut)

Rendering donut charts in .NET MAUI just got a whole lot easier!

EZCharts.Maui.Donut is a control library built on top of [SkiaSharp](https://github.com/mono/SkiaSharp), dedicated to creating a developer friendly and feature rich cross-platform donut chart component. The goal is to provide developers with a highly customizable, efficient, and visually appealing donut chart view that they can implement into their applications with minimal setup.

## 🖼️ Samples
A [sample project](/EZCharts.Maui.Donut.Samples) can be found in the repository where you can dive deeper into setup, customisation and how to use the library in a typical MAUI application. There are samples for MVVM, code behind and XAML setups.

More detailed samples and documentation coming soon!

![Sample Animation](https://raw.githubusercontent.com/DashTheDev/EZCharts.Maui.Donut/master/Media/Sample.gif)

## 🔧 Setting Up
1. Install package via [NuGet](https://www.nuget.org/packages/EZCharts.Maui.Donut).
2. Add `UseDonutChart()` to your `CreateMauiApp()` in `MauiProgram.cs`.

    ```cs
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseDonutChart()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }
    ```

3. Add the `xmlns` namespace and `DonutChartView` to your XAML view. 

    ```xaml
    <YourView xmlns:donut="http://schemas.dashthedev.com/ez-charts/maui/donut">
        <donut:DonutChartView />
    </YourView>
    ```

4. Add entry models (your own or our generic class) via binding, code-behind or XAML. Your choice!

    #### Binding
    ```xaml
    <donut:DonutChartView
        EntriesSource="{Binding TestResults}"
        EntryLabelPath="Category"
        EntryValuePath="Score" />
    ```

    #### Code-behind
    ```xaml
    <donut:DonutChartView x:Name="MyChartView" />
    ```
    
    ```cs
    public SamplePage()
    {
        InitializeComponent();

        MyChartView.EntriesSource = new DataEntry[]
        {
            new()
            {
                Value = 105,
                Label = "Pencils Owned"
            },
            new()
            {
                Value = 234,
                Label = "Pens Owned"
            },
        };
    }
    ```

    #### XAML
    ```xaml
    <donut:DonutChartView>
        <x:Array Type="{x:Type donut:DataEntry}">
            <donut:DataEntry Label="English" Value="200" />
            <donut:DataEntry Label="Mathematics" Value="300" />
            <donut:DataEntry Label="Geography" Value="325" />
            <donut:DataEntry Label="Science" Value="50" />
        </x:Array>
    </donut:DonutChartView>
    ```

5. Customise to your liking! Options and samples can be found in the [documentation](/) (coming soon).

## 🤝 Contributing
I work full-time and may not have time to keep things up to date. So if there's something you want to change, then make some contributions! Please read the [contribution guide](/) (coming soon) on how to get started.

Any contributions are greatly appreciated. :)