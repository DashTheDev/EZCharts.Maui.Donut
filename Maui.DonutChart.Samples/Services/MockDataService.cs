﻿using Maui.DonutChart.Samples.Models;

namespace Maui.DonutChart.Samples.Services;

internal class MockDataService
{
    #region Fields

    private readonly float _minValue = 15f;
    private readonly float _maxValue = 200f;
    private readonly int _minResultCount = 1;
    private readonly int _maxResultCount = 10;
    private readonly string[] _categories = 
    [
        "English",
        "Mathematics",
        "Science",
        "Geography",
        "History",
        "Technology",
        "Sports",
        "Music",
        "Drama",
        "Languages"
    ];

    #endregion

    #region Service Methods

    internal TestResult[] GetTestResults()
    {
        Random random = new();
        int resultCount = random.Next(_minResultCount, _maxResultCount);
        List<TestResult> testResults = [];

        for (int i = 0; i < resultCount; i++)
        {
            testResults.Add(new TestResult()
            {
                Score = GetRandomScore(random),
                Category = _categories[i]
            });
        }

        return [.. testResults];
    }

    #endregion

    #region Supporting Methods

    private float GetRandomScore(Random random)
        => (float)(_minValue + (random.NextDouble() * (_maxValue - _minValue)));

    #endregion
}