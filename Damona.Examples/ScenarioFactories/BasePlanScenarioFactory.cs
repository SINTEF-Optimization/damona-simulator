using System.Globalization;
using Damona.Schemas.AutoMapper;
using Damona.Simulator;
using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Resources;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Examples.ScenarioFactories;

/// <summary>
///     Scenario factory for the base scenario used in the user tests in October 2023.
/// </summary>
public class BasePlanScenarioFactory(SpecialtyMap specialtyMap) : IScenarioFactory
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;
    protected List<int> _exclude = new();

    public IEnumerable<OperatingTheatre> Create(Simulation environment, DateTime fromTime, DateTime toTime)
    {
        var theatres = new[]
            {
                "theatre-1 - emergencies",
                "theatre-2",
                "theatre-3",
                "theatre-4",
                "theatre-5",
                "theatre-6",
                "theatre-7",
                "theatre-8",
                "theatre-9",
                "theatre-10",
                "theatre-11",
                "theatre-12",
                "theatre-13",
            }.Select(name => new OperatingTheatre(environment, name))
            .ToList();

        PopulateEmergencyTheatre(theatres[0], fromTime, toTime);
        PopulateFullyOccupiedTheatre(theatres[1], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-1"));
        PopulateFullyOccupiedTheatre(theatres[2], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-6"));
        PopulateTheatre3(theatres[3], fromTime, toTime);
        PopulateTheatre4(theatres[4], fromTime, toTime);
        PopulateFullyOccupiedTheatre(theatres[5], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-3"));
        PopulateFullyOccupiedTheatre(theatres[6], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-3"));
        PopulateFullyOccupiedTheatre(theatres[7], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-8"));
        PopulateFullyOccupiedTheatre(theatres[8], fromTime, toTime, _specialtyMap.GetOrAdd("specialty-2"));
        PopulateTheatre10(theatres[9], fromTime, toTime);
        PopulateTheatre11(theatres[10], fromTime, toTime);
        PopulateTheatre12(theatres[11], fromTime, toTime);
        PopulateTheatre13(theatres[12], fromTime, toTime);

        var excludedTheatres = _exclude.Select(idx => theatres[idx]).ToList();
        foreach (var excluded in excludedTheatres)
            theatres.Remove(excluded);

        return theatres;
    }

    private void PopulateEmergencyTheatre(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        // Assume the emergency is always open
        theatre.Allocation.NewEntry(fromTime, toTime - fromTime, Constants.EmergencyId);
    }

    private void PopulateFullyOccupiedTheatre(
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

    private void PopulateTheatre3(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            // Monday
            {
                var fromTimeMonday = OpenFrom(date);
                var toTimeMonday = OpenTo(date);
                var halfTimeMonday = 0.5 * (toTimeMonday - fromTimeMonday);

                theatre.Allocation.NewEntry(
                    date.Add(fromTimeMonday),
                    halfTimeMonday,
                    _specialtyMap.GetOrAdd("specialty-4"));
                theatre.Allocation.NewEntry(
                    date.Add(fromTimeMonday + halfTimeMonday),
                    halfTimeMonday,
                    _specialtyMap.GetOrAdd("specialty-5"));
            }

            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-5"));
            AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-5"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-4"));

            date = date.AddDays(7);
        }
    }

    private void PopulateTheatre4(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-2"));
            date = date.AddDays(7);
        }
    }

    private void PopulateTheatre10(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-4"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-7"));
            AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-7"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-8"));

            date = date.AddDays(7);
        }
    }

    private void PopulateTheatre11(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(2), _specialtyMap.GetOrAdd("specialty-7"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(4), _specialtyMap.GetOrAdd("specialty-3"));

            date = date.AddDays(7);
        }
    }

    private void PopulateTheatre12(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            AddFullDayEntry(theatre, date, _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(1), _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(3), _specialtyMap.GetOrAdd("specialty-3"));
            AddFullDayEntry(theatre, date.AddDays(4), _specialtyMap.GetOrAdd("specialty-3"));

            date = date.AddDays(7);
        }
    }

    private void PopulateTheatre13(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        var date = fromTime.MondayCurrentWeek().Date;
        var isOddWeek = ISOWeek.GetWeekOfYear(date) % 2 == 1;

        while (date < toTime)
        {
            if (!isOddWeek)
            {
                AddFullDayEntry(theatre, date.AddDays(4), _specialtyMap.GetOrAdd("specialty-9"));
            }

            isOddWeek = !isOddWeek;
            date = date.AddDays(7);
        }
    }

    private void AddFullDayEntry(OperatingTheatre theatre, DateTime date, Specialty specialty)
    {
        var openFrom = OpenFrom(date);
        var openTo = OpenTo(date);
        theatre.Allocation.NewEntry(date.Add(openFrom), openTo - openFrom, specialty);
    }

    private TimeSpan OpenFrom(DateTime date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday or DayOfWeek.Tuesday => new(7, 40, 0),
            DayOfWeek.Wednesday => new(8, 50, 0),
            DayOfWeek.Thursday => new(8, 40, 0),
            DayOfWeek.Friday => new(8, 45, 0), // TODO: first friday every month
            _ => TimeSpan.Zero
        };
    }

    private TimeSpan OpenTo(DateTime date)
    {
        return new(15, 30, 0);
    }
}