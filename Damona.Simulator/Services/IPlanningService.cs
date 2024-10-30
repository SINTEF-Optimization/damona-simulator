using Damona.Simulator.Processes.Planning;

namespace Damona.Simulator.Services;

/// <summary>
///     Creates and modifies a surgery schedule based on new arrivals and waiting lists. Used by
///     <see cref="PlanningProcess"/> to update the plan.
/// </summary>
public interface IPlanningService
{
    void ScheduledPlanning();

    void ReplanOnWaitingListUpdate();

    void ReplanOnEmergencyListUpdate();
}