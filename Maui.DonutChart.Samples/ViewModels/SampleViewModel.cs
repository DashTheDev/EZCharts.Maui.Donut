using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Maui.DonutChart.Samples.Models;
using Maui.DonutChart.Samples.Services;

namespace Maui.DonutChart.Samples.ViewModels;

internal sealed partial class SampleViewModel : BaseViewModel
{
    private readonly MockDataService _mockDataService;

    public SampleViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
        RefreshData();
    }

    public ObservableCollection<TestResult> TestResults { get; private set; } = [];

    [RelayCommand]
    private void RefreshData()
    {
        TestResults = [.. _mockDataService.GetTestResults()];
    }
}