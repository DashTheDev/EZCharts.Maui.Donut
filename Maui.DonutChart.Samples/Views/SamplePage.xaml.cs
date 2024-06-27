namespace Maui.DonutChart.Samples.Views;

public partial class SamplePage : ContentPage
{
    public SamplePage()
    {
        InitializeComponent();
    }

    private void OnEntryClicked(object sender, float e)
    {
        ClickedLabel.Text = $"Entry with value {e}, clicked!";
    }
}

