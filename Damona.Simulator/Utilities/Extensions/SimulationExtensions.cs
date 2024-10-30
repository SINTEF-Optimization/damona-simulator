using SimSharp;

namespace Damona.Simulator.Utilities;

public static class SimulationExtensions
{
    /// <summary>
    ///     Creates and returns a timeout that waits until a specified time.
    /// </summary>
    /// <param name="until">The datetime the timeout is fired.</param>
    /// <param name="priority">The priority to rank events at the same time (smaller value = higher priority).</param>
    /// <returns>The scheduled timeout event that was created.</returns>
    public static SimSharp.Timeout WaitUntil(this Simulation env, DateTime until, int priority = 0)
    {
        // TODO: Error if until is before Now?
        return env.Timeout(until - env.Now, priority);
    }
}