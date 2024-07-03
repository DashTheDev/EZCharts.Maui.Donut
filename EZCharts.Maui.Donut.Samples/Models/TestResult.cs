namespace EZCharts.Maui.Donut.Samples.Models;

public class TestResult
{
    public float Score { get; set; }
    public ResultCategory Category { get; set; }
    public string CategoryDisplay => Category.ToString();
}