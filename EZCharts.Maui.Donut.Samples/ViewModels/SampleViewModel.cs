using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using EZCharts.Maui.Donut.Samples.Models;
using EZCharts.Maui.Donut.Samples.Services;

namespace EZCharts.Maui.Donut.Samples.ViewModels;

internal sealed partial class SampleViewModel : BaseViewModel
{
    #region Fields

    private readonly MockDataService _mockDataService;

    #endregion

    #region Constructor

    public SampleViewModel(MockDataService mockDataService)
    {
        _mockDataService = mockDataService;
        RefreshData();
    }

    #endregion

    #region Bindable Properties

    public ObservableList<TestResult> TestResults { get; private set; } = [];

    #endregion

    #region Commands

    [RelayCommand]
    private static Task EntryClicked(object entry)
    {
        if (entry is not TestResult testResult)
        {
            return Task.CompletedTask;
        }

        string displayText = $"Entry \"{testResult.Category}\" with value {testResult.Score}, clicked!";
        IToast toast = Toast.Make(displayText, ToastDuration.Short, 14);
        return toast.Show();
    }

    [RelayCommand]
    private void RefreshData()
    {
        TestResults.Clear();
        TestResults.AddRange(_mockDataService.GetTestResults());
    }

    #endregion
}