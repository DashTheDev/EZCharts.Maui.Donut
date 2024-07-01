﻿using CommunityToolkit.Maui;
using Maui.DonutChart.Samples.Services;
using Maui.DonutChart.Samples.ViewModels;
using Maui.DonutChart.Samples.Views;
using Microsoft.Extensions.Logging;

namespace Maui.DonutChart.Samples;

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
