using Damona.Simulator.Domain;
using Damona.Simulator.Resources;

namespace Damona.Simulator.Monitors;

/// <summary>
///     A <see cref="Surgery"/> that was executed in the simulation.
/// </summary>
public class ExecutedSurgery<TSurgery>
    where TSurgery : Surgery
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required TSurgery Surgery { get; set; }
    public required OperatingTheatre? OperatingTheatre { get; set; }
}