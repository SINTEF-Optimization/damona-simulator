namespace Damona.Simulator.Utilities;

public static class DateTimeExtensions
{
    public static DateTime StartOfNextMonth(this DateTime dateTime)
    {
        var year = dateTime.Year;
        var month = dateTime.Month + 1;

        if (month == 13)
        {
            year += 1;
            month = 1;
        }

        return new(
            year,
            month,
            1,
            dateTime.Hour,
            dateTime.Minute,
            dateTime.Second,
            dateTime.Millisecond,
            dateTime.Microsecond);
    }

    public static DateTime StartOfNextMonth(this DateTime dateTime, TimeOnly time)
    {
        var year = dateTime.Year;
        var month = dateTime.Month + 1;

        if (month == 13)
        {
            year += 1;
            month = 1;
        }

        return new(year, month, 1, time.Hour, time.Minute, time.Second, time.Millisecond, time.Microsecond);
    }

    public static DateTime NextMonday(this DateTime dateTime)
    {
        return dateTime.AddDays(
            dateTime.DayOfWeek switch
            {
                DayOfWeek.Sunday => 1,
                DayOfWeek.Monday => 7,
                DayOfWeek.Tuesday => 6,
                DayOfWeek.Wednesday => 5,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 3,
                DayOfWeek.Saturday => 2,
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static DateTime MondayCurrentWeek(this DateTime dateTime)
    {
        return dateTime.AddDays(
            dateTime.DayOfWeek switch
            {
                DayOfWeek.Sunday => -6,
                DayOfWeek.Monday => 0,
                DayOfWeek.Tuesday => -1,
                DayOfWeek.Wednesday => -2,
                DayOfWeek.Thursday => -3,
                DayOfWeek.Friday => -4,
                DayOfWeek.Saturday => -5,
                _ => throw new ArgumentOutOfRangeException()
            });
    }


    public static bool IsOnDay(this DateTime dateTime, DateOnly date)
    {
        return dateTime.Year == date.Year && dateTime.Month == date.Month && dateTime.Day == date.Day;
    }

    public static bool IsBeforeDay(this DateTime dateTime, DateOnly date)
    {
        if (dateTime.Year == date.Year)
        {
            if (dateTime.Month == date.Month)
                return dateTime.Day < date.Day;

            return dateTime.Month < date.Month;
        }

        return dateTime.Year < date.Year;
    }

    public static bool IsAfterDay(this DateTime dateTime, DateOnly date)
    {
        if (dateTime.Year == date.Year)
        {
            if (dateTime.Month == date.Month)
                return dateTime.Day < date.Day;

            return dateTime.Month < date.Month;
        }

        return dateTime.Year < date.Year;
    }
}