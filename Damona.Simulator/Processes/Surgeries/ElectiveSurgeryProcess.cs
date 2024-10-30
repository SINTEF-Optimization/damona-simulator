using System.Collections;
using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Resources;
using SimSharp;

namespace Damona.Simulator.Processes.Surgeries;

/// <summary>
///     An <see cref="ISurgeryProcess"/> that executes an <see cref="ElectiveSurgery"/>. It requests the
///     <see cref="OperatingTheatre"/> resource, updates the waiting list and <see cref="ISurgeryMonitor{TSurgery}"/>,
///     and frees the resource when the surgery is over including the 20 minute prep time between surgeries.
/// </summary>
public class ElectiveSurgeryProcess : ISurgeryProcess, IEnumerable<Event>
{
    private readonly Simulation _environment;
    private readonly IWaitingList _waitingList;
    private readonly ISurgeryMonitor<ElectiveSurgery>? _monitor;

    private readonly ElectiveSurgery _surgery;
    private readonly OperatingTheatre _operatingTheatre;

    public ElectiveSurgeryProcess(
        Simulation environment,
        IWaitingList waitingList,
        ISurgeryMonitor<ElectiveSurgery>? monitor,
        ElectiveSurgery surgery,
        OperatingTheatre operatingTheatre)
    {
        _environment = environment;
        _waitingList = waitingList;
        _monitor = monitor;
        _surgery = surgery;
        _operatingTheatre = operatingTheatre;

        _waitingList.RemoveElectiveSurgery(_surgery);
    }

    public IEnumerator<Event> GetEnumerator()
    {
        return Process().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IEnumerable<Event> Process()
    {
        using var request = _operatingTheatre.Request();
        yield return request;

        var startTime = _environment.Now;

        // TODO: Stochastic surgery duration
        yield return _environment.Timeout(_surgery.EstimatedDuration);

        _monitor?.AddCompleted(_surgery, startTime, _environment.Now, _operatingTheatre);

        // TODO: Parametrize the surgery separation time
        yield return _environment.Timeout(TimeSpan.FromMinutes(20));
    }
}