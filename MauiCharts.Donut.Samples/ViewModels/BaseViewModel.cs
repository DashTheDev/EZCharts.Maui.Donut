using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCharts.Donut.Samples.Services;

namespace MauiCharts.Donut.Samples.ViewModels;

internal abstract partial class BaseViewModel : ObservableObject
{
    [RelayCommand]
    private static Task GoBack()
    {
        return NavigationService.GoBackAsync();
    }
}