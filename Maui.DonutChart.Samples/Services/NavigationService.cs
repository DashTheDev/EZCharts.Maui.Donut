using Maui.DonutChart.Samples.ViewModels;

namespace Maui.DonutChart.Samples.Services;

internal static class NavigationService
{
    internal static Dictionary<Type, Type> ViewModelToViews { get; } = [];

    internal static Task GoToAsync<TViewModel>() where TViewModel : BaseViewModel
    {
        string viewName = GetViewName<TViewModel>();
        return Shell.Current.GoToAsync(viewName, true);
    }

    internal static Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..", true);
    }

    private static string GetViewName<TViewModel>()
    {
        Type viewModelType = typeof(TViewModel);

        if (!ViewModelToViews.TryGetValue(viewModelType, out Type? viewType))
        {
            throw new InvalidOperationException($"{viewModelType.Name} is not registered for navigation.");
        }

        return viewType.Name;
    }
}