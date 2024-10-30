using Damona.Simulator.Domain;
using Damona.Simulator.Resources;
using SimSharp;
using static Damona.Simulator.Tests.SpecialtyIds;
using static Damona.Simulator.Constants;

namespace Damona.Simulator.Tests.Resources;

public class HospitalTests
{
    private static readonly DateTime StartTime = new(2023, 1, 1);

    [Fact(DisplayName = "WhenAny yields at the opening hour of the hospital when one theatre")]
    public void WhenAny_OneTheatreNoSurgeries_YieldsWhenOpeningHours()
    {
        // Arrange:
        var env = new Simulation();
        var hospital = CreateSingleTheatreHospital(env, OrthopedicsId, 7);

        IEnumerable<Event> TestProcess()
        {
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(1));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(2));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAny yields immediately when not setting change only")]
    public void WhenAny_NotChangeOnly_OneTheatre()
    {
        // Arrange:
        var env = new Simulation();
        var hospital = CreateSingleTheatreHospital(env, OrthopedicsId, 7);

        IEnumerable<Event> TestProcess()
        {
            yield return hospital.WhenAny(changeOnly: false);
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));
            yield return hospital.WhenAny(changeOnly: false);
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));
            yield return hospital.WhenAny(changeOnly: false);
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));

            yield return hospital.WhenAny(changeOnly: true);
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(1));
            yield return hospital.WhenAny(changeOnly: false);
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(1));
            yield return hospital.WhenAny(changeOnly: false);
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(1));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAny yields at the opening hour of each theatre when three theatres")]
    public void WhenAny_ThreeTheatresNoSurgeries_YieldsWhenOpeningHours()
    {
        // Arrange:
        var env = new Simulation();
        var hospital = CreateThreeTheatreHospital(env, 7);

        IEnumerable<Event> TestProcess()
        {
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(4));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(8));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(12));

            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(4) + TimeSpan.FromDays(1));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(1));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(8) + TimeSpan.FromDays(1));
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(12) + TimeSpan.FromDays(1));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAny yields at the opening hour and when surgery is over")]
    public void WhenAny_OneTheatreAndSurgeries_YieldsWhenOpeningHoursOrSurgeryOver()
    {
        // Arrange:
        var env = new Simulation();
        var hospital = CreateSingleTheatreHospital(env, OrthopedicsId, 7);

        IEnumerable<Event> SurgeryProcess()
        {
            using var request = hospital.OperatingTheatres[0].Request();
            yield return request;
            yield return env.Timeout(TimeSpan.FromMinutes(46));
        }

        IEnumerable<Event> TestProcess()
        {
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));

            var proc = env.Process(SurgeryProcess());
            yield return hospital.WhenAny();
            proc.IsProcessed.Should().BeTrue();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromMinutes(46));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAny yields when surgery over and opening hours when three theatres")]
    public void WhenAny_ThreeTheatreAndOneSurgery_YieldsImmediately()
    {
        // Arrange:
        var env = new Simulation();
        var hospital = CreateThreeTheatreHospital(env, 7);

        IEnumerable<Event> SurgeryProcess(int index)
        {
            using var request = hospital.OperatingTheatres[index].Request();
            yield return request;
            yield return env.Timeout(TimeSpan.FromMinutes(46));
        }

        IEnumerable<Event> TestProcess()
        {
            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(4));

            var proc1 = env.Process(SurgeryProcess(1));
            yield return hospital.WhenAny();
            proc1.IsProcessed.Should().BeTrue();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(4) + TimeSpan.FromMinutes(46));

            var proc2 = env.Process(SurgeryProcess(1));
            yield return hospital.WhenAny();
            proc2.IsProcessed.Should().BeTrue();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(4) + 2 * TimeSpan.FromMinutes(46));

            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAny yields when surgery is over if surgery request before opening hours")]
    public void WhenAny_ThreeTheatreAndSurgeryRequestBeforeOpening()
    {
        // Arrange:
        var env = new Simulation();
        var hospital = CreateThreeTheatreHospital(env, 7);

        IEnumerable<Event> SurgeryProcess()
        {
            using var request = hospital.OperatingTheatres[0].Request();
            yield return request;
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6));
            yield return env.Timeout(TimeSpan.FromMinutes(46));
        }

        IEnumerable<Event> TestProcess()
        {
            var proc = env.Process(SurgeryProcess());
            yield return hospital.WhenAny();
            proc.IsProcessed.Should().BeFalse();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(4));

            yield return hospital.WhenAny();
            proc.IsProcessed.Should().BeTrue();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(6) + TimeSpan.FromMinutes(46));

            yield return hospital.WhenAny();
            env.Now.Should().Be(StartTime + TimeSpan.FromHours(8));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    private Hospital CreateSingleTheatreHospital(Simulation env, Specialty specialty, int numDays)
    {
        var theatres = new OperatingTheatre[] { new(env) };

        for (var day = 0; day < numDays; day++)
            theatres[0]
                .Allocation.NewEntry(
                    StartTime + TimeSpan.FromHours(6) + TimeSpan.FromDays(day),
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
            theatres[0]
                .Allocation.NewEntry(startTime + TimeSpan.FromHours(6), TimeSpan.FromHours(6), OrthopedicsId);
            theatres[0].Allocation.NewEntry(startTime + TimeSpan.FromHours(12), TimeSpan.FromHours(6), OdontologyId);
            theatres[1]
                .Allocation.NewEntry(
                    startTime + TimeSpan.FromHours(4),
                    TimeSpan.FromHours(10),
                    EarNoseThroatId);
            theatres[2]
                .Allocation.NewEntry(startTime + TimeSpan.FromHours(8), TimeSpan.FromHours(14), EmergencyId);
        }

        return new(env, theatres);
    }
}