using CommunityToolkit.Mvvm.Input;
using MauiCharts.Donut.Samples.Services;

namespace MauiCharts.Donut.Samples.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel
{
    [RelayCommand]
    private static Task ViewSample()
    {
        return NavigationService.GoToAsync<SampleViewModel>();
    }
}