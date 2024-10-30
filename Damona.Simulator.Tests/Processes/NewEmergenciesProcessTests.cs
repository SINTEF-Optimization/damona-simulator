using Damona.Simulator.Distributions;
using Damona.Simulator.Domain;
using Damona.Simulator.Plans;
using Damona.Simulator.Processes.Emergencies;
using SimSharp;
using static Damona.Simulator.Constants;
using static Damona.Simulator.Tests.SpecialtyIds;

namespace Damona.Simulator.Tests.Processes;

public class NewEmergenciesProcessTests
{
    private readonly EmergencyList _emergencyList = new();
    private readonly Mock<INewEmergenciesDistribution> _distribution = new();

    private readonly DateTime StartTime = new(2023, 1, 1);

    public NewEmergenciesProcessTests()
    {
        var emergencyEnumerator = Emergencies().GetEnumerator();
        _distribution.Setup(m => m.Next())
            .Returns(
                () =>
                {
                    if (!emergencyEnumerator.MoveNext())
                        return null;

                    return emergencyEnumerator.Current;
                });
    }

    [Fact(DisplayName = "Updates the emergency list, and that is handled immediately (sim time)")]
    public void UpdatesEmergencyList()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var sut = new NewEmergenciesProcess(env, _emergencyList, _distribution.Object);

        var times = SubscribeToNewEmergencies(env);

        // Act:
        env.Run(TimeSpan.FromDays(30));

        // Assert:
        times.Should().BeEquivalentTo(Emergencies().Select(e => e.EmergencyDate));
    }

    private IEnumerable<Emergency> Emergencies()
    {
        return new Emergency[]
        {
            new()
            {
                EmergencyDate = StartTime + TimeSpan.FromHours(1),
                Urgency = (Urgency)1,
                Specialty = EmergencyId,
                EstimatedDuration = default
            },
            new()
            {
                EmergencyDate = StartTime + TimeSpan.FromHours(2),
                Urgency = (Urgency)1,
                Specialty = OdontologyId,
                EstimatedDuration = default
            },
            new()
            {
                EmergencyDate = StartTime + TimeSpan.FromDays(1) + TimeSpan.FromHours(0),
                Urgency = (Urgency)1,
                Specialty = OrthopedicsId,
                EstimatedDuration = default
            }
        };
    }

    private List<DateTime> SubscribeToNewEmergencies(Simulation env)
    {
        var simulationTimes = new List<DateTime>();
        _emergencyList.NewEmergency += delegate { simulationTimes.Add(env.Now); };

        return simulationTimes;
    }
}