using Damona.Simulator.Plans;
using Damona.Simulator.Processes.Planning;
using Damona.Simulator.Services;
using SimSharp;

namespace Damona.Simulator.Tests.Processes;

public class PlanningProcessTests
{
    private readonly Mock<WaitingList> _waitingList = new();
    private readonly Mock<EmergencyList> _emergencyList = new();
    private readonly Mock<SurgerySchedule> _surgerySchedule = new();
    private readonly Mock<IPlanningService> _planningService = new();

    private readonly DateTime StartTime = new(2023, 01, 01);

    [Fact(DisplayName = "Calls the planning service the expected number of times when only scheduled planning")]
    void CallsPlanningServiceWhenScheduledOnly()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var sut = new PlanningProcess(
            env,
            _waitingList.Object,
            _emergencyList.Object,
            _surgerySchedule.Object,
            _planningService.Object);

        // Act:
        env.Run(env.StartDate + TimeSpan.FromDays(100));

        // Assert:
        _planningService.Verify(
            m => m.ScheduledPlanning(),
            Times.Exactly(15),
            "There are 15 planning sessions in 100 days when plan is updated every week (incl beginning)");
    }
}