using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Processes.Operations;
using Damona.Simulator.Resources;
using SimSharp;
using static Damona.Simulator.Constants;
using static Damona.Simulator.Tests.SpecialtyIds;

namespace Damona.Simulator.Tests.Processes.Operations;

public class ExecuteWaitingListOperationProcessTests
{
    private static readonly DateTime StartTime = new(2023, 1, 1);
    private readonly Mock<IWaitingList> _waitingList = new();
    private readonly Mock<IEmergencyList> _emergencyList = new();


    [Fact(DisplayName = "Runs and does nothing when no surgeries")]
    public void Simulate_RunsAndDoesNothingWhenNoSurgeries()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var electiveSurgeryMonitor = new SurgeryMonitor<ElectiveSurgery>(env, collect: true);
        var emergencyMonitor = new SurgeryMonitor<Emergency>(env, collect: true);
        var hospital = CreateSingleTheatreHospital(env, OrthopedicsId, 7);
        var sut = new ExecuteWaitingListOperationProcess(
            env,
            _waitingList.Object,
            _emergencyList.Object,
            hospital);

        sut.ElectiveSurgeryMonitor = electiveSurgeryMonitor;
        sut.EmergencyMonitor = emergencyMonitor;

        _emergencyList.Setup(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>())).Returns<Emergency?>(null!);
        _waitingList.Setup(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>())).Returns<ElectiveSurgery?>(null!);

        // Act:
        env.Run();
        electiveSurgeryMonitor.Completed.Should().BeEmpty();
        emergencyMonitor.Completed.Should().BeEmpty();
        _emergencyList.Verify(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>()), Times.Exactly(7));
        _waitingList.Verify(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>()), Times.Exactly(7));
    }

    [Fact(DisplayName = "Executes emergencies first when there is always an emergency")]
    public void Simulate_EmergenciesFirst()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var electiveSurgeryMonitor = new SurgeryMonitor<ElectiveSurgery>(env, collect: true);
        var emergencyMonitor = new SurgeryMonitor<Emergency>(env, collect: true);
        var hospital = CreateSingleTheatreHospital(env, OrthopedicsId, 7);
        var sut = new ExecuteWaitingListOperationProcess(
            env,
            _waitingList.Object,
            _emergencyList.Object,
            hospital);

        sut.ElectiveSurgeryMonitor = electiveSurgeryMonitor;
        sut.EmergencyMonitor = emergencyMonitor;

        var emergencies = new[]
        {
            NewEmergency(OrthopedicsId, TimeSpan.FromMinutes(57)),
            NewEmergency(OrthopedicsId, TimeSpan.FromMinutes(33))
        };
        var emergencyGenerator = emergencies.AsEnumerable().GetEnumerator();

        _emergencyList.Setup(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>()))
            .Returns<Specialty, TimeSpan>((_, _) => emergencyGenerator.MoveNext() ? emergencyGenerator.Current : null);

        var electiveSurgeries = new[]
        {
            NewElectiveSurgery(OrthopedicsId, TimeSpan.FromMinutes(10)),
            NewElectiveSurgery(OrthopedicsId, TimeSpan.FromMinutes(88)),
        };
        var electiveSurgeryGenerator = electiveSurgeries.AsEnumerable().GetEnumerator();

        _waitingList.Setup(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>()))
            .Returns<Specialty, TimeSpan>(
                (_, _) => electiveSurgeryGenerator.MoveNext() ? electiveSurgeryGenerator.Current : null);

        // Act:
        env.Run();

        // NOTE: Remember 20 minute offset
        emergencyMonitor.Completed.Should()
            .BeEquivalentTo(
                new ExecutedSurgery<Emergency>[]
                {
                    new()
                    {
                        StartTime = StartTime.AddHours(6),
                        EndTime = StartTime.AddHours(6).AddMinutes(57),
                        Surgery = emergencies[0],
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(57 + 20),
                        EndTime = StartTime.AddHours(6).AddMinutes(57 + 20 + 33),
                        Surgery = emergencies[1],
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                },
                options => options.ComparingByValue<OperatingTheatre>());

        electiveSurgeryMonitor.Completed.Should()
            .BeEquivalentTo(
                new ExecutedSurgery<ElectiveSurgery>[]
                {
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(57 + 33 + 2 * 20),
                        EndTime = StartTime.AddHours(6).AddMinutes(57 + 33 + 2 * 20 + 10),
                        Surgery = electiveSurgeries[0],
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(57 + 33 + 10 + 3 * 20),
                        EndTime = StartTime.AddHours(6).AddMinutes(57 + 33 + 10 + 3 * 20 + 88),
                        Surgery = electiveSurgeries[1],
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                },
                options => options.ComparingByValue<OperatingTheatre>());
    }

    [Fact(DisplayName = "Executes elective surgery when break in emergencies")]
    public void Simulate_ElectiveSurgeriesWhenNoEmergencies()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var electiveSurgeryMonitor = new SurgeryMonitor<ElectiveSurgery>(env, collect: true);
        var emergencyMonitor = new SurgeryMonitor<Emergency>(env, collect: true);
        var hospital = CreateSingleTheatreHospital(env, OrthopedicsId, 7);
        var sut = new ExecuteWaitingListOperationProcess(
            env,
            _waitingList.Object,
            _emergencyList.Object,
            hospital);
        sut.EmergencyMonitor = emergencyMonitor;
        sut.ElectiveSurgeryMonitor = electiveSurgeryMonitor;

        var emergencies = new[]
        {
            null,
            NewEmergency(OrthopedicsId, TimeSpan.FromMinutes(57)),
            null,
            NewEmergency(OrthopedicsId, TimeSpan.FromMinutes(33))
        };
        var emergencyGenerator = emergencies.AsEnumerable().GetEnumerator();

        _emergencyList.Setup(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>()))
            .Returns<Specialty, TimeSpan>((_, _) => emergencyGenerator.MoveNext() ? emergencyGenerator.Current : null);

        var electiveSurgeries = new[]
        {
            NewElectiveSurgery(OrthopedicsId, TimeSpan.FromMinutes(10)),
            NewElectiveSurgery(OrthopedicsId, TimeSpan.FromMinutes(88)),
        };
        var electiveSurgeryGenerator = electiveSurgeries.AsEnumerable().GetEnumerator();

        _waitingList.Setup(m => m.Next(It.IsAny<Specialty>(), It.IsAny<TimeSpan>()))
            .Returns<Specialty, TimeSpan>(
                (_, _) => electiveSurgeryGenerator.MoveNext() ? electiveSurgeryGenerator.Current : null);

        // Act:
        env.Run();

        // NOTE: Remember 20 minute offset
        emergencyMonitor.Completed.Should()
            .BeEquivalentTo(
                new ExecutedSurgery<Emergency>[]
                {
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(10 + 20),
                        EndTime = StartTime.AddHours(6).AddMinutes(10 + 20 + 57),
                        Surgery = emergencies[1]!,
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(10 + 57 + 88 + 3 * 20),
                        EndTime = StartTime.AddHours(6).AddMinutes(10 + 57 + 88 + 3 * 20 + 33),
                        Surgery = emergencies[3]!,
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                },
                options => options.ComparingByValue<OperatingTheatre>());

        electiveSurgeryMonitor.Completed.Should()
            .BeEquivalentTo(
                new ExecutedSurgery<ElectiveSurgery>[]
                {
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(0),
                        EndTime = StartTime.AddHours(6).AddMinutes(10),
                        Surgery = electiveSurgeries[0],
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                    new()
                    {
                        StartTime = StartTime.AddHours(6).AddMinutes(10 + 57 + 2 * 20),
                        EndTime = StartTime.AddHours(6).AddMinutes(10 + 57 + 2 * 20 + 88),
                        Surgery = electiveSurgeries[1],
                        OperatingTheatre = hospital.OperatingTheatres[0]
                    },
                },
                options => options.ComparingByValue<OperatingTheatre>());
    }

    private Hospital CreateSingleTheatreHospital(Simulation env, Specialty specialty, int numDays)
    {
        var theatres = new OperatingTheatre[] { new(env) };

        for (var day = 0; day < numDays; day++)
            theatres[0]
                .Allocation.NewEntry(
                    StartTime.AddDays(day).AddHours(6),
                    TimeSpan.FromHours(12),
                    specialty);

        return new(env, theatres);
    }

    private Hospital CreateThreeTheatreHospital(Simulation env, int numDays)
    {
        var theatres = new OperatingTheatre[] { new(env), new(env), new(env) };

        for (var day = 0; day < numDays; day++)
        {
            var startTime = StartTime + TimeSpan.FromDays(day);
            theatres[0].Allocation.NewEntry(startTime.AddHours(6), TimeSpan.FromHours(6), OrthopedicsId);
            theatres[0].Allocation.NewEntry(startTime.AddHours(12), TimeSpan.FromHours(6), OdontologyId);
            theatres[1].Allocation.NewEntry(startTime.AddHours(4), TimeSpan.FromHours(10), EarNoseThroatId);
            theatres[2].Allocation.NewEntry(startTime.AddHours(8), TimeSpan.FromHours(14), EmergencyId);
        }

        return new(env, theatres);
    }

    private static ElectiveSurgery NewElectiveSurgery(Specialty specialty) =>
        NewElectiveSurgery(specialty, TimeSpan.FromHours(1));

    private static ElectiveSurgery NewElectiveSurgery(Specialty specialty, TimeSpan duration) =>
        new()
        {
            Specialty = specialty,
            EstimatedDuration = duration,
        };

    private static Emergency NewEmergency(Specialty specialty, Urgency urgency = (Urgency)1) =>
        NewEmergency(specialty, TimeSpan.FromHours(1), urgency);

    private static Emergency NewEmergency(Specialty specialty, TimeSpan duration, Urgency urgency = (Urgency)1) =>
        new()
        {
            Urgency = urgency,
            Specialty = specialty,
            EstimatedDuration = duration,
            EmergencyDate = StartTime
        };
}