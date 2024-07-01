using CommunityToolkit.Mvvm.Input;
using EZCharts.Maui.Donut.Samples.Services;

namespace EZCharts.Maui.Donut.Samples.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel
{
    [RelayCommand]
    private static Task ViewSample()
    {
        return NavigationService.GoToAsync<SampleViewModel>();
    }
}