using Damona.Simulator.Domain;

namespace Damona.Simulator.Monitors.Reports;

public class SurgeryReport : Dictionary<Specialty, SurgeryReport.Entry>
{
    public void AddInitial(Specialty specialty, int count = 1)
    {
        var entry = GetEntryOrNew(specialty);
        entry.Initial += count;

        if (entry.Periods.Any())
            entry.Periods[0].Initial += count;
    }

    public void AddNew(Specialty specialty, DateTime date, int count = 1)
    {
        var entry = GetEntryOrNew(specialty);
        var period = GetPeriodEntryOrNew(entry, date);

        entry.New += count;
        period.New += count;
    }

    public void AddCompleted(Specialty specialty, DateTime date, int count = 1)
    {
        var entry = GetEntryOrNew(specialty);
        var period = GetPeriodEntryOrNew(entry, date);

        entry.Completed += count;
        period.Completed += count;
    }

    public Entry GetEntryOrNew(Specialty specialty)
    {
        TryAdd(specialty, new());
        return this[specialty];
    }

    public PeriodEntry GetPeriodEntryOrNew(Specialty specialty, DateTime date)
    {
        var entry = GetEntryOrNew(specialty);
        return GetPeriodEntryOrNew(entry, date);
    }

    private PeriodEntry GetPeriodEntryOrNew(Entry entry, DateTime date)
    {
        var period = entry.Periods.LastOrDefault();

        var month = new DateOnly(date.Year, date.Month, 1);
        if (period is null)
        {
            period = new()
            {
                Month = month,
                Initial = entry.Initial
            };

            entry.Periods.Add(period);
            return period;
        }

        if (period.Month == month)
            return period;

        var previous = (PeriodEntry?)null;
        if (entry.Periods.Count > 0)
            previous = entry.Periods[^1];

        if (previous is not null && previous.Month > month)
            throw new ArgumentException("Updating the report in non-chronological order");

        period = new()
        {
            Month = month,
            Initial = previous?.Final ?? 0
        };

        entry.Periods.Add(period);
        return period;
    }


    public class PeriodEntry
    {
        public DateOnly Month { get; set; }
        public int Initial { get; set; }
        public int New { get; set; }
        public int Completed { get; set; }
        public int Final => Initial + New - Completed;
    }

    public class Entry
    {
        public int Initial { get; set; }
        public int New { get; set; }
        public int Completed { get; set; }
        public int Final => Initial + New - Completed;

        public List<PeriodEntry> Periods { get; } = new();
    }
}