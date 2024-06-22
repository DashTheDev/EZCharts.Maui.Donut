using System.Collections.ObjectModel;

namespace Maui.DonutChart.Controls;

public class DataEntryCollection : ObservableCollection<DataEntry>
{
    public DataEntryCollection() { }
    public DataEntryCollection(IEnumerable<DataEntry> collection) : base(collection) { }
}