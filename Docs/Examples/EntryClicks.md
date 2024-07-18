# [⮪](README.md) Processing Entry Clicks
Once you've [setup](../../README.md#-setting-up) your [`DonutChartView`](../Reference/DonutChartView.md) you can react to segment clicks on the chart via [`ICommand`](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/commanding?view=net-maui-8.0#icommands) or an [`event`](https://learn.microsoft.com/en-us/dotnet/standard/events/#events). Both approaches receive an [`object`](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) that represents the segment's associated entry class instance which is set by the underlying [`EntriesSource`](../Reference/DonutChartView.md) so that you can do what you need with the data.

## Command Approach
On your [`DonutChartView`](../Reference/DonutChartView.md) you can bind the delegate you want to execute on click to the [`ICommand`](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/commanding?view=net-maui-8.0#icommands) property, [`EntryClickedCommand`](../Reference/DonutChartView.md). As mentioned previously, this command will receive an [`object`](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) parameter which is the clicked entry.

### MVVM
In this example we're using a [ViewModel](https://learn.microsoft.com/en-us/dotnet/maui/xaml/fundamentals/mvvm?view=net-maui-8.0#commanding) setup with the [`CommunityToolkit.MVVM`](https://www.nuget.org/packages/CommunityToolkit.Mvvm) package to provide our entry clicked delegate which receives the clicked entry and casts to the type `TestResult` via [pattern matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/tutorials/safely-cast-using-pattern-matching-is-and-as-operators) and displays its category and value as a [toast](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/alerts/toast?tabs=windows%2Candroid).

```C#
internal sealed partial class SampleViewModel : BaseViewModel
{
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
}
```

#### XAML
```XAML
<donut:DonutChartView EntriesSource="{Binding TestResults}" EntryClickedCommand="{Binding EntryClickedCommand}" />
```

#### Code-Behind
```C#
MyChartView.EntryClickedCommand = viewModel.EntryClickedCommand;
```

### Non-MVVM
```C#
private void OnEntryClicked(object entry)
{
    if (entry is not TestResult testResult)
    {
        return;
    }

    System.Diagnostics.Debug.WriteLine($"Entry \"{testResult.CategoryDisplay}\" with value {testResult.Score}, clicked!");
}

MyChartView.EntryClickedCommand = new Command(OnEntryClicked);
```

## Event Approach
On your [`DonutChartView`](../Reference/DonutChartView.md) you can subscribe to the entry clicks and provide the delegate you want to execute on click to the [`event`](https://learn.microsoft.com/en-us/dotnet/standard/events/#events), [`EntryClicked`](../Reference/DonutChartView.md). As mentioned previously, this event will receive an [`object`](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) argument which is the clicked entry.

Since both the XAML and Code-Behind approach will rely on the delegate being defined in the code-behind. Picture the following method setup in the code-behind.

```C#
private void OnEntryClicked(object? sender, EntryClickEventArgs e)
{
    if (e.Entry is not TestResult testResult)
    {
        return;
    }

    System.Diagnostics.Debug.WriteLine($"Entry \"{testResult.CategoryDisplay}\" with value {testResult.Score}, clicked!");
}
```

### XAML
```XAML
<donut:DonutChartView EntriesSource="{Binding TestResults}" EntryClicked="OnEntryClicked" />
```

### Code-Behind
```C#
MyChartView.EntryClicked += OnEntryClicked;
```

> [!NOTE]
> You should ensure that you manage subscriptions to the [`EntryClicked`](../Reference/DonutChartView.md) [`event`](https://learn.microsoft.com/en-us/dotnet/standard/events/#events) in an optimal way to reduce the risk of memory leaks in your app. [Read more here](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-subscribe-to-and-unsubscribe-from-events).