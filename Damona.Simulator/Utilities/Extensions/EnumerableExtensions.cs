using SimSharp;

namespace Damona.Simulator.Utilities.Extensions;

public static class EnumerableExtensions
{
    public static T RandomElement<T>(this IEnumerable<T> items, IRandom random)
    {
        return items.ElementAt(random.Next(0, items.Count()));
    }

    public static T RandomElement<T>(this IEnumerable<T> items, Random random)
    {
        return items.ElementAt(random.Next(0, items.Count()));
    }

    public static bool IsEmpty<T>(this IEnumerable<T> items)
    {
        return !items.Any();
    }
}