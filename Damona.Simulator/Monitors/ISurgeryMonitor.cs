using Damona.Simulator.Domain;
using Damona.Simulator.Monitors.Reports;
using Damona.Simulator.Resources;

namespace Damona.Simulator.Monitors;

/// <summary>
///     Simulation monitor that records the surgeries executed in the simulation.
/// </summary>
/// <typeparam name="TSurgery"></typeparam>
public interface ISurgeryMonitor<TSurgery> where TSurgery : Surgery
{
    /// <summary>
    ///     Register that an <typeparamref name="TSurgery"/> was added to its waiting list
    /// </summary>
    void AddNew(TSurgery surgery);

    /// <summary>
    ///     Register that a <typeparamref name="TSurgery"/> was completed.
    /// </summary>
    /// <param name="surgery">The surgery that was executed.</param>
    /// <param name="startTime">When the surgery started</param>
    /// <param name="endTime">When the surgery ended</param>
    /// <param name="theatre">Which operating theatre the surgery was carried out in</param>
    void AddCompleted(TSurgery surgery, DateTime startTime, DateTime endTime, OperatingTheatre theatre);

    List<ExecutedSurgery<TSurgery>> Completed { get; }

    SurgeryReport Report();
}