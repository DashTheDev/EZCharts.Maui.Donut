using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EZCharts.Maui.Donut.Samples.Models;

// Original: https://github.com/dotnet/maui/blob/c02195d36fe520758f48afccd221fccbb8a2f7b4/src/Controls/src/Core/ObservableList.cs#L10-L14
// TODO: Track https://github.com/dotnet/maui/pull/20036 and https://github.com/dotnet/runtime/issues/18087.
internal class ObservableList<T> : ObservableCollection<T>, IRangeList<T>
{
    public ObservableList() { }

    public ObservableList(IEnumerable<T> collection) : base(collection) { }

    /// <inheritdoc/>
    public void AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range, nameof(range));

        List<T> items = range.ToList();
        int index = Items.Count;

        foreach (T item in items)
        {
            Items.Add(item);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, index));
    }

    /// <inheritdoc/>
    public void InsertRange(int index, IEnumerable<T> range)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(Count, 0, nameof(index));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count, nameof(index));
        ArgumentNullException.ThrowIfNull(Count, nameof(range));

        int originalIndex = index;
        List<T> items = range.ToList();

        foreach (T item in items)
        {
            Items.Insert(index++, item);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, originalIndex));
    }

    /// <inheritdoc/>
    public void Move(int oldIndex, int newIndex, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(oldIndex, 0, nameof(oldIndex));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(oldIndex + count, Count, nameof(oldIndex));
        ArgumentOutOfRangeException.ThrowIfLessThan(newIndex, 0, nameof(newIndex));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(newIndex + count, Count, nameof(newIndex));

        List<T> items = new(count);
        for (var i = 0; i < count; i++)
        {
            T item = Items[oldIndex];
            items.Add(item);
            Items.RemoveAt(oldIndex);
        }

        int index = newIndex;
        if (newIndex > oldIndex)
        {
            index -= items.Count - 1;
        }

        for (var i = 0; i < items.Count; i++)
        {
            Items.Insert(index + i, items[i]);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, items, newIndex, oldIndex));
    }

    /// <inheritdoc/>
    public void RemoveAt(int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(index));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(index + count, Count, nameof(index));

        T[] items = Items.Skip(index).Take(count).ToArray();
        for (int i = index; i < count; i++)
        {
            Items.RemoveAt(i);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, index));
    }

    /// <inheritdoc/>
    public void RemoveRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range, nameof(range));

        List<T> items = range.ToList();
        foreach (T item in items)
        {
            Items.Remove(item);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
    }

    /// <inheritdoc/>
    public void ReplaceRange(int startIndex, IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items, nameof(items));

        T[] ritems = items.ToArray();

        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, 0, nameof(startIndex));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex + ritems.Length, Count, nameof(startIndex));

        T[] oldItems = new T[ritems.Length];
        for (var i = 0; i < ritems.Length; i++)
        {
            oldItems[i] = Items[i + startIndex];
            Items[i + startIndex] = ritems[i];
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, ritems, oldItems, startIndex));
    }
}

/// <summary>
/// Represents a list that has support for bulk operations.
/// </summary>
internal interface IRangeList<T> : IList<T>
{
    /// <summary>
    /// Add the provided <paramref name="range"/> to the list.
    /// </summary>
    void AddRange(IEnumerable<T> range);

    /// <summary>
    /// Insert the provided <paramref name="range"/> to the list at the provided <paramref name="index"/>.
    /// </summary>
    void InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Move the <paramref name="count"/> of <typeparamref name="T"/> elements from <paramref name="oldIndex"/> to <paramref name="newIndex"/>.
    /// </summary>
    void Move(int oldIndex, int newIndex, int count);

    /// <summary>
    /// Remove the <paramref name="count"/> of <typeparamref name="T"/> elements at the provided <paramref name="index"/>.
    /// </summary>
    void RemoveAt(int index, int count);

    /// <summary>
    /// Remove the <paramref name="range"/> from the list.
    /// </summary>
    void RemoveRange(IEnumerable<T> range);

    /// <summary>
    /// Replace the <typeparamref name="T"/> element at <paramref name="startIndex"/> with the provided <paramref name="items"/>.
    /// </summary>
    void ReplaceRange(int startIndex, IEnumerable<T> items);
}