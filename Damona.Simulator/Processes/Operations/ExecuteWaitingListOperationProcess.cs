using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Processes.Surgeries;
using Damona.Simulator.Resources;
using SimSharp;
using static Damona.Simulator.Constants;

namespace Damona.Simulator.Processes.Operations;

/// <summary>
///     Process executed surgeries by assigning them to <see cref="OperatingTheatre"/>.
/// </summary>
public class ExecuteWaitingListOperationProcess : ActiveObject<Simulation>, IOperationsProcess
{
    private readonly IWaitingList _waitingList;
    private readonly IEmergencyList _emergencyList;
    private readonly Hospital _hospital;

    public ISurgeryMonitor<Emergency>? EmergencyMonitor { get; set; }
    public ISurgeryMonitor<ElectiveSurgery>? ElectiveSurgeryMonitor { get; set; }

    public ExecuteWaitingListOperationProcess(
        Simulation environment,
        IWaitingList waitingList,
        IEmergencyList emergencyList,
        Hospital hospital)
        : base(environment)
    {
        _waitingList = waitingList;
        _emergencyList = emergencyList;
        _hospital = hospital;

        Environment.Process(ProcessQueue());
    }

    public IEnumerable<Event> ProcessQueue()
    {
        var first = true;
        while (true)
        {
            // NOTE: Specify change only on subsequent calls so we don't end in an infinite request loop
            yield return _hospital.WhenAny(changeOnly: !first);
            first = false;

            var availableOts = _hospital.OperatingTheatres.Where(x => x.IsAvailable).ToList();
            if (availableOts.Count == 0)
                continue;

            QueueEmergencies(availableOts);
            QueueElectiveSurgeries(availableOts);
        }
    }

    /// <summary>
    ///     Queue the next emergency in the <see cref="IEmergencyList"/> using the first <see cref="OperatingTheatre"/>
    ///     that is available to take the specific surgery.
    /// </summary>
    /// <param name="availableOts">The <see cref="OperatingTheatre"/> that are currently available.</param>
    /// <remarks>
    ///     When an operating theatre is queued, it is removed from the list of currently available theatres.
    /// </remarks>
    private void QueueEmergencies(List<OperatingTheatre> availableOts)
    {
        for (var index = 0; index < availableOts.Count; index++)
        {
            if (availableOts[index].SpecialtyAtTime() != EmergencyId)
                continue;

            var ot = availableOts[index];
            availableOts.RemoveAt(index);
            index--;

            var specialty = ot.SpecialtyAtTime();
            var availableTime = ot.RemainingTime();

            if (specialty is null || availableTime == TimeSpan.Zero)
                continue;

            var emergency = _emergencyList.Next(specialty.Value, availableTime);
            if (emergency is null)
                continue;

            Environment.Process(new EmergencyProcess(Environment, _emergencyList, EmergencyMonitor, emergency, ot));
        }

        for (var index = 0; index < availableOts.Count; index++)
        {
            var ot = availableOts[index];

            var specialty = ot.SpecialtyAtTime();
            var availableTime = ot.RemainingTime();

            if (specialty is null || availableTime == TimeSpan.Zero)
                continue;

            var emergency = _emergencyList.Next(specialty.Value, availableTime);
            if (emergency is null)
                continue;

            Environment.Process(new EmergencyProcess(Environment, _emergencyList, EmergencyMonitor, emergency, ot));

            availableOts.RemoveAt(index);
            index--;
        }
    }

    /// <summary>
    ///     Queue the next elective surgery in the <see cref="IWaitingList"/> using the first
    ///     <see cref="OperatingTheatre"/> that is available to take the specific surgery.
    /// </summary>
    /// <param name="availableOts">The <see cref="OperatingTheatre"/> that are currently available.</param>
    private void QueueElectiveSurgeries(List<OperatingTheatre> availableOts)
    {
        foreach (var ot in availableOts)
        {
            var specialty = ot.SpecialtyAtTime();
            var availableTime = ot.RemainingTime();

            if (specialty is null || availableTime == TimeSpan.Zero)
                continue;

            var electiveSurgery = _waitingList.Next(specialty.Value, availableTime);
            if (electiveSurgery is null)
                continue;

            Environment.Process(
                new ElectiveSurgeryProcess(Environment, _waitingList, ElectiveSurgeryMonitor, electiveSurgery, ot));
        }
    }
}