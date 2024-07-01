# MauiCharts.Donut

![DonutChart Logo](path/to/logo.png)

## 🍩 About MauiCharts.Donut

MauiCharts.Donut is a focused fork of the [MicroCharts](https://github.com/microcharts-dotnet/Microcharts) library, dedicated to creating a robust and feature-rich donut chart component for .NET MAUI applications. Our goal is to provide developers with a highly customizable, efficient, and visually appealing donut chart solution.

## 🚀 Recent Improvements

In our latest update, we've made changes to the core functionality of the DonutChart:

- **Simplified Sector Rendering**: Improved the `CreateSectorPath` method in `SKGeometry.cs` for more efficient chart rendering.
- **Enhanced Degree Calculation**: Added a new `GetDegrees` method in `CircleConstants.cs` for more accurate angle calculations.
- **Refactored DonutChartView**: Updated the `RenderData` method in `DonutChartView.cs` to utilize the new sector path creation logic.

These changes lay the groundwork for more advanced features and optimisations in future releases.

## 🎯 Roadmap

Our vision for MauiCharts.Donut includes:

- [ ] Implementing advanced hit-testing for improved interactivity
- [ ] Adding animation support for smooth data transitions
- [ ] Introducing customizable themes and styles
- [ ] Optimizing performance for large datasets
- [ ] Enhancing accessibility features

## 📦 Installation

```bash
dotnet add package MauiCharts.Donut# MauiCharts.Donut

🔧 Usage
Here's a quick example of how to use MauiCharts.Donut in your XAML:

<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:sc="clr-namespace:MauiCharts.Donut;assembly=MauiCharts.Donut"
             x:Class="YourNamespace.MainPage">

    <sc:DonutChartView Data="{Binding YourData}"
                       Colors="{Binding YourColors}"
                       StrokeWidth="2"
                       StrokeColor="White" />

</ContentPage>
```
🤝 Contributing
We welcome contributions! If you'd like to help improve MauiCharts.Donut, please:

Fork the repository
Create your feature branch (git checkout -b feature/AmazingFeature)
Commit your changes (git commit -m 'Add some AmazingFeature')
Push to the branch (git push origin feature/AmazingFeature)
Open a Pull Request

📄 License
This project is licensed under the MIT License - see the LICENSE.md file for details.
📬 Contact
Dash -
Project Link: https://github.com/Dash/MauiCharts.Donut
