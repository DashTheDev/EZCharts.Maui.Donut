using CommunityToolkit.Maui;
using MauiCharts.Donut.Samples.Services;
using MauiCharts.Donut.Samples.ViewModels;
using MauiCharts.Donut.Samples.Views;
using Microsoft.Extensions.Logging;

namespace MauiCharts.Donut.Samples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseDonutChart()
            .RegisterViews()
            .RegisterServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        #if DEBUG
		builder.Logging.AddDebug();
        #endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterViews(this MauiAppBuilder appBuilder)
    {
        appBuilder.Services
            .RegisterView<MainPage, MainViewModel>()
            .RegisterView<SamplePage, SampleViewModel>();

        return appBuilder;
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder appBuilder)
    {
        appBuilder.Services.AddSingleton<MockDataService>();
        return appBuilder;
    }
}
