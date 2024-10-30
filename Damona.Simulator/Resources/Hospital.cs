using SimSharp;

namespace Damona.Simulator.Resources;

/// <summary>
///     A hospital is a SimSharp resource that holds a set of operating theatres with opening hours and specialty
///     allocations.
/// </summary>
public class Hospital
{
    public List<OperatingTheatre> OperatingTheatres { get; }

    public Simulation Environment { get; }

    public Hospital(Simulation environment, IEnumerable<OperatingTheatre> operatingTheatres)
    {
        Environment = environment;
        OperatingTheatres = operatingTheatres.ToList();
    }

    /// <summary>
    ///     Event that triggers when any of the operating theatres are available.
    /// </summary>
    /// <param name="changeOnly">Only trigger when there is a change. I.e. do not trigger from this request.</param>
    public Event WhenAny(bool changeOnly = true)
    {
        // PERF: Will this result in a lot of unused events?
        return new AnyOf(Environment, OperatingTheatres.Select(x => x.WhenAvailable(changeOnly)));
    }
}