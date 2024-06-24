using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.DonutChart.Samples.Services;

namespace Maui.DonutChart.Samples.ViewModels;

internal abstract partial class BaseViewModel : ObservableObject
{
    [RelayCommand]
    private static Task GoBack()
    {
        return NavigationService.GoBackAsync();
    }
}