using Damona.Simulator.Plans;
using Damona.Simulator.Processes.Operations;
using Damona.Simulator.Services;
using SimSharp;

namespace Damona.Simulator.Processes.Planning;

/// <summary>
///     Process that handles the plan and updating the plan when new things happen to the waiting lists. It uses a
///     <see cref="IPlanningService"/> to create the plans and sequence the patients.
/// </summary>
/// <remarks>
///     This is not included in the current simulation as <see cref="ExecuteWaitingListOperationProcess"/> instead
///     schedules the surgeries on a first-come-first-serve basis.
/// </remarks>
public class PlanningProcess : ActiveObject<Simulation>
{
    private readonly WaitingList _waitingList;
    private readonly EmergencyList _emergencyList;
    private readonly SurgerySchedule _surgerySchedule;
    private readonly IPlanningService _planningService;

    private readonly TimeSpan _planningFrequency = TimeSpan.FromDays(7);

    public PlanningProcess(
        Simulation env,
        WaitingList waitingList,
        EmergencyList emergencyList,
        SurgerySchedule surgerySchedule,
        IPlanningService planningService)
        : base(env)
    {
        _waitingList = waitingList;
        _emergencyList = emergencyList;
        _surgerySchedule = surgerySchedule;
        _planningService = planningService;

        _waitingList.Updated += OnWaitingListUpdated;
        _emergencyList.NewEmergency += OnNewEmergency;

        env.Process(ScheduledPlanning());
    }

    public IEnumerable<Event> ScheduledPlanning()
    {
        // TODO: Initial planning
        // TODO: Wait until next "monday"
        while (true)
        {
            _planningService.ScheduledPlanning();
            Environment.Log($"New scheduled plan {Environment.Now}");
            yield return Environment.Timeout(_planningFrequency);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    private void OnWaitingListUpdated(object? sender, EventArgs eventArgs)
    {
        _planningService.ReplanOnWaitingListUpdate();
    }

    private void OnNewEmergency(object? sender, EventArgs eventArgs)
    {
        _planningService.ReplanOnEmergencyListUpdate();
    }
}