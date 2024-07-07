# [⮪](README.md) EntryClickEventArgs
An [EventArgs](https://learn.microsoft.com/en-us/dotnet/api/system.eventargs?view=net-8.0) class used to provide arguments for the [`DonutChartView.EntryClicked`](DonutChartView.md) event.

```C#
public sealed class EntryClickEventArgs(object entry) : EventArgs
```

**Inheritance:** [Object](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) -> [EventArgs](https://learn.microsoft.com/en-us/dotnet/api/system.eventargs?view=net-8.0) -> EntryClickEventArgs

## Properties
| Name | Type | Description |
|:-:|:-:|:-:|
| Entry | [`object`](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) | The data entry [`object`](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) that was clicked. The type of this [`object`](https://learn.microsoft.com/en-us/dotnet/api/system.object?view=net-8.0) will be the same as the entry types bound to the [`DonutChartView.EntriesSource`](DonutChartView.md). |