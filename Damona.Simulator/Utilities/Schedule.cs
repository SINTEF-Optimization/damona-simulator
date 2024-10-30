using Damona.Simulator.Resources;

namespace Damona.Simulator.Utilities;

/// <summary>
///     A schedule entries which is a non-overlapping series of timespans with an associated value. Used to specify the
///     allocation schedule for an <see cref="OperatingTheatre"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Schedule<T>
{
    private SortedList<DateTime, Entry> _entries = new();

    public IList<Entry> Entries => _entries.Values;

    public Schedule()
    {
    }

    /// <summary>
    ///     Add an entry to the schedule.
    /// </summary>
    public void AddEntry(Entry entry)
    {
        // TODO: Throw if it overlaps another entry
        _entries.Add(entry.StartTime, entry);
    }

    /// <summary>
    ///     Add an entry to the schedule.
    /// </summary>
    public void NewEntry(DateTime startTime, TimeSpan duration, T value)
    {
        AddEntry(new(startTime, duration, value));
    }


    /// <summary>
    ///     Get the entry at a specific datetime.
    /// </summary>
    /// <param name="time">The time to find the entry for.</param>
    /// <returns>The entry at that time, <c>null</c> if no timespans overlap with <paramref name="time"/>.</returns>
    public Entry? EntryAt(DateTime time)
    {
        foreach (var (_, entry) in _entries)
        {
            if (entry.EndTime <= time)
                continue;

            if (entry.StartTime > time)
                break;

            return entry;
        }

        return null;
    }

    /// <summary>
    ///     An entry in the schedule.
    /// </summary>
    public record Entry(DateTime StartTime, TimeSpan Duration, T Value)
    {
        public DateTime EndTime => StartTime + Duration;
    }
}