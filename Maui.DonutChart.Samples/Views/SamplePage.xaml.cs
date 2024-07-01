using Maui.DonutChart.Models;
using Maui.DonutChart.Samples.Models;

namespace Maui.DonutChart.Samples.Views;

public partial class SamplePage : ContentPage
{
    public SamplePage()
    {
        InitializeComponent();
    }

    private void OnEntryClicked(object sender, EntryClickEventArgs e)
    {
        float? value = null;
        string? label = null;

        if (e.Entry is TestResult testResult)
        {
            value = testResult.Score;
            label = testResult.Category;
        }
        else if (e.Entry is DataEntry dataEntry)
        {
            value = dataEntry.Value;
            label = dataEntry.Label;
        }

        if (value is not null && label is not null)
        {
            ClickedLabel.Text = $"Entry \"{label}\" with value {value}, clicked!";
        }
        else
        {
            ClickedLabel.Text = "Unknown entry clicked!";
        }
    }
}

