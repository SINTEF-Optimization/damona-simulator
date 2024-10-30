using System.Collections;
using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Resources;
using SimSharp;

namespace Damona.Simulator.Processes.Surgeries;

/// <summary>
///     An <see cref="ISurgeryProcess"/> that executes an <see cref="Emergency"/>. It requests the
///     <see cref="OperatingTheatre"/> resource, updates the emergency list and <see cref="ISurgeryMonitor{TSurgery}"/>,
///     and frees the resource when the surgery is over including the 20 minute prep time between surgeries.
/// </summary>
public class EmergencyProcess : ISurgeryProcess, IEnumerable<Event>
{
    private readonly Simulation _environment;
    private readonly IEmergencyList _emergencyList;
    private readonly ISurgeryMonitor<Emergency>? _monitor;

    private readonly Emergency _emergency;
    private readonly OperatingTheatre _operatingTheatre;

    public EmergencyProcess(
        Simulation environment,
        IEmergencyList emergencyList,
        ISurgeryMonitor<Emergency>? monitor,
        Emergency emergency,
        OperatingTheatre operatingTheatre)
    {
        _environment = environment;
        _emergency = emergency;
        _emergencyList = emergencyList;
        _operatingTheatre = operatingTheatre;
        _monitor = monitor;

        _emergencyList.RemoveEmergency(_emergency);
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
        yield return _environment.Timeout(_emergency.EstimatedDuration);

        _monitor?.AddCompleted(_emergency, startTime, _environment.Now, _operatingTheatre);

        // TODO: Parametrize the surgery separation time
        yield return _environment.Timeout(TimeSpan.FromMinutes(20));
    }
}