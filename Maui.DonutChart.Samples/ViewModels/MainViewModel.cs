using CommunityToolkit.Mvvm.Input;
using Maui.DonutChart.Samples.Services;

namespace Maui.DonutChart.Samples.ViewModels;

internal sealed partial class MainViewModel : BaseViewModel
{
    [RelayCommand]
    private static Task ViewSample()
    {
        return NavigationService.GoToAsync<SampleViewModel>();
    }
}