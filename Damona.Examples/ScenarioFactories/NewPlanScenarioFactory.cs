using System.Globalization;
using Damona.Schemas.AutoMapper;
using Damona.Simulator;
using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Resources;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Examples.ScenarioFactories;

/// <summary>
///     Scenario factory for the adjusted scenario used in the user tests in October 2023 with only 9 theatres.
/// </summary>
public class NewPlanScenarioFactory(SpecialtyMap specialtyMap) : IScenarioFactory
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;

    public IEnumerable<OperatingTheatre> Create(Simulation environment, DateTime fromTime, DateTime toTime)
    {
        var theatres = Enumerable.Repeat(() => new OperatingTheatre(environment), 9).Select(x => x()).ToList();

        PopulateTheatre1(theatres[0], fromTime, toTime);
        PopulateTheatre2(theatres[1], fromTime, toTime);
        PopulateTheatre3(theatres[2], fromTime, toTime);
        PopulateFullyOccupiedTheatre(theatres[3], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-2"));
        PopulateFullyOccupiedTheatre(theatres[4], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-3"));
        PopulateTheatre6(theatres[5], fromTime, toTime);
        PopulateFullyOccupiedTheatre(theatres[6], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-3"));
        PopulateTheatre8(theatres[7], fromTime, toTime);
        PopulateFullyOccupiedTheatre(theatres[8], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-8"));

        return theatres;
    }

    protected virtual void PopulateFullyOccupiedTheatre(
        OperatingTheatre theatre,
        DateTime fromTime,
        DateTime toTime,
        Specialty specialty)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            for (var day = 0; day < 5; day++)
                AddFullDayEntry(theatre, date.AddDays(day), specialty);

            date = date.AddDays(7);
        }
    }

    protected virtual void PopulateTheatre1(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        var isOddWeek = ISOWeek.GetWeekOfYear(date) % 2 == 1;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-1"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-1"));
            AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-1"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-1"));

            // Friday
            {
                var fromTimeFriday = OpenFrom(date.AddDays(4));
                var toTimeFriday = OpenTo(date.AddDays(4));
                var halfTimeFriday = 0.5 * (toTimeFriday - fromTimeFriday);

                theatre.Allocation.NewEntry(
                    date.AddDays(4),
                    halfTimeFriday,
                    isOddWeek ? _specialtyMap.GetOrAdd("specialty-1") : _specialtyMap.GetOrAdd("specialty-6"));
            }

            isOddWeek = !isOddWeek;
            date = date.AddDays(7);
        }
    }

    protected virtual void PopulateTheatre2(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-6"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-6"));
            AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-6"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-6"));

            date = date.AddDays(7);
        }
    }

    protected virtual void PopulateTheatre3(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        var isOddWeek = ISOWeek.GetWeekOfYear(date) % 2 == 1;

        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-5"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-5"));

            if (isOddWeek)
                AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-9"));

            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-4"));
            AddFullDayEntry(theatre, date.AddDays(4), _specialtyMap.GetOrAdd("specialty-4"));

            isOddWeek = !isOddWeek;
            date = date.AddDays(7);
        }
    }

    private void PopulateTheatre6(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        theatre.Allocation.NewEntry(fromTime, toTime - fromTime, Constants.EmergencyId);
    }

    protected virtual void PopulateTheatre8(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date.AddDays(0), _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-7"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-7"));
            AddFullDayEntry(theatre, date.AddDays(4), _specialtyMap.GetOrAdd("specialty-7"));

            date = date.AddDays(7);
        }
    }

    protected void AddFullDayEntry(OperatingTheatre theatre, DateTime date, Specialty specialty)
    {
        var openFrom = OpenFrom(date);
        var openTo = OpenTo(date);
        theatre.Allocation.NewEntry(date.Add(openFrom), openTo - openFrom, specialty);
    }

    protected virtual TimeSpan OpenFrom(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Tuesday => new(7, 40, 0),
            DayOfWeek.Wednesday => new(8, 50, 0),
            DayOfWeek.Thursday => new(8, 40, 0),
            DayOfWeek.Friday => new(8, 45, 0),
            _ => TimeSpan.Zero
        };
    }

    protected virtual TimeSpan OpenTo(DateTime date)
    {
        return new(15, 30, 0);
    }
}