using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EZCharts.Maui.Donut.Samples.Services;

namespace EZCharts.Maui.Donut.Samples.ViewModels;

internal abstract partial class BaseViewModel : ObservableObject
{
    [RelayCommand]
    private static Task GoBack()
    {
        return NavigationService.GoBackAsync();
    }
}