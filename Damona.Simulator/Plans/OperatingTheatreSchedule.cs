using Damona.Simulator.Domain;
using Damona.Simulator.Resources;

namespace Damona.Simulator.Plans;

public class OperatingTheatreSchedule
{
    public OperatingTheatre OperatingTheatre { get; }

    private SortedList<DateTime, PlannedSurgery> _entries = new();

    public DateTime Now { get; private set; }

    public IList<PlannedSurgery> Entries => _entries.Values;

    public OperatingTheatreSchedule(DateTime startTime)
    {
        Now = startTime;
    }

    public void AddEntry(PlannedSurgery entry)
    {
        // TODO: Throw if it overlaps another entry

        if (entry.Time < Now)
            throw new ArgumentException(
                $"Schedule has been executed until {Now}, cannot add entry that starts at {entry.Time}");

        _entries.Add(entry.Time, entry);
    }

    public void NewEntry(DateTime time, TimeSpan duration, ElectiveSurgery electiveSurgery)
    {
        AddEntry(new(time, duration, electiveSurgery));
    }

    public IEnumerable<PlannedSurgery> SurgeriesOnDay(DateOnly date)
    {
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        foreach (var (_, entry) in _entries)
        {
            if (entry.Time <= endOfDay)
                continue;

            if (entry.Time > endOfDay)
                break;

            yield return entry;
        }
    }

    // public TimeSpan OccupiedTimeOnDay(DateOnly date)
    // {
    //     return EntriesOnDay(date)
    //         .Aggregate(new TimeSpan(), (current, entry) => current + (entry.To - entry.From));
    // }

    /// <summary>
    ///     Execute the current schedule until the specified time. This pops all surgeries schedules until this time,
    ///     and updated the <see cref="Now"/> variable of the schedule.
    /// </summary>
    /// <param name="until">When to execute the schedule until.</param>
    /// <returns>All surgeries to be completed between previous <see cref="Now"/> and <paramref name="until"/></returns>
    public IEnumerable<PlannedSurgery> ExecuteUntil(DateTime until)
    {
        var entries = new List<PlannedSurgery>();

        while (true)
        {
            if (!_entries.Any())
                break;

            var time = _entries.GetKeyAtIndex(0);
            if (time > until)
                break;

            var entry = _entries.GetValueAtIndex(0);
            entries.Add(entry);
            _entries.RemoveAt(0);
        }

        Now = until;
        return entries;
    }
}