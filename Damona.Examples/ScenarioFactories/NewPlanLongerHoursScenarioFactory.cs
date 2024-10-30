using System.Globalization;
using Damona.Schemas.AutoMapper;
using Damona.Simulator.Resources;
using Damona.Simulator.Utilities;

namespace Damona.Examples.ScenarioFactories;

/// <summary>
///     Adjusted <see cref="NewPlanScenarioFactory"/> where theatre 1 has longer opening hours. 
/// </summary>
public class NewPlanLongerHoursScenarioFactory(SpecialtyMap specialtyMap, TimeSpan extraTime)
    : NewPlanScenarioFactory(specialtyMap)
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;
    private readonly TimeSpan _extraTime = extraTime;

    protected override void PopulateTheatre1(OperatingTheatre theatre, DateTime fromTime, DateTime toTime)
    {
        const int increaseHoursForWeeks = 16;

        var date = fromTime.MondayCurrentWeek().Date;
        var isOddWeek = ISOWeek.GetWeekOfYear(date) % 2 == 1;
        var weekNumber = 0;
        var extraTime = _extraTime;

        while (date < toTime)
        {
            if (weekNumber == increaseHoursForWeeks)
                extraTime = TimeSpan.Zero;

            AddFullDayEntry(theatre, date, extraTime, _specialtyMap.GetOrAdd("specialty-1"));
            AddFullDayEntry(theatre, date.AddDays(1), extraTime, _specialtyMap.GetOrAdd("specialty-1"));
            AddFullDayEntry(theatre, date.AddDays(2), extraTime, _specialtyMap.GetOrAdd("specialty-1"));
            AddFullDayEntry(theatre, date.AddDays(3), extraTime, _specialtyMap.GetOrAdd("specialty-1"));

            // Friday
            {
                var fromTimeFriday = OpenFrom(date.AddDays(4));
                var toTimeFriday = OpenTo(date.AddDays(4));
                var halfTimeFriday = 0.5 * (toTimeFriday - fromTimeFriday);

                theatre.Allocation.NewEntry(
                    date.AddDays(4),
                    halfTimeFriday + (isOddWeek ? extraTime : TimeSpan.Zero),
                    isOddWeek ? _specialtyMap.GetOrAdd("specialty-1") : _specialtyMap.GetOrAdd("specialty-6"));
            }

            weekNumber++;
            isOddWeek = !isOddWeek;
            date = date.AddDays(7);
        }
    }

    private void AddFullDayEntry(OperatingTheatre theatre, DateTime date, TimeSpan extraTime, Specialty specialty)
    {
        var openFrom = OpenFrom(date);
        var openTo = OpenTo(date);
        theatre.Allocation.NewEntry(date.Add(openFrom), (openTo - openFrom) + extraTime, specialty);
    }
}