using Maui.DonutChart.Samples.Models;

namespace Maui.DonutChart.Samples.Services;

internal class MockDataService
{
    private readonly float _minValue = 0f;
    private readonly float _maxValue = 200f;
    private readonly int _minResultCount = 1;
    private readonly int _maxResultCount = 10;

    internal TestResult[] GetTestResults()
    {
        Random random = new();
        int resultCount = random.Next(_minResultCount, _maxResultCount);
        List<TestResult> testResults = [];

        for (int i = 0; i < resultCount; i++)
        {
            testResults.Add(new TestResult()
            {
                Score = GetRandomScore(random)
            });
        }

        return [.. testResults];
    }

    private float GetRandomScore(Random random)
        => (float)(_minValue + (random.NextDouble() * (_maxValue - _minValue)));
}