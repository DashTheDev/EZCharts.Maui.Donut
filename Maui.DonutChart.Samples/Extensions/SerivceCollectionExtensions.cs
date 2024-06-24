using Maui.DonutChart.Samples.Services;
using Maui.DonutChart.Samples.ViewModels;

namespace Microsoft.Extensions.DependencyInjection;

internal static class SerivceCollectionExtensions
{
    internal static IServiceCollection RegisterView<TView, TViewModel>(this IServiceCollection services)
        where TView : Page, new()
        where TViewModel : BaseViewModel
    {
        Type pageType = typeof(TView);

        services.AddTransient<TViewModel>();
        services.AddTransient(typeof(TView), serviceProvider =>
        {
            TViewModel viewModel = serviceProvider.GetRequiredService<TViewModel>();
            return new TView()
            {
                BindingContext = viewModel
            };
        });

        NavigationService.ViewModelToViews.Add(typeof(TViewModel), pageType);
        Routing.RegisterRoute(pageType.Name, pageType);
        return services;
    }
}