using Maui.DonutChart.Controls;

namespace Maui.DonutChart.Samples.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnEntryClicked(object sender, EventArgs e)
    {
        if (sender is not DataEntry entry)
        {
            return;
        }

        ClickedLabel.Text = $"Entry with value {entry.Value}, clicked!";
    }
}

