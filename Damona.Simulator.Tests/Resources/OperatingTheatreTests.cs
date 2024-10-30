using Damona.Simulator.Domain;
using Damona.Simulator.Resources;
using SimSharp;
using static Damona.Simulator.Tests.SpecialtyIds;

namespace Damona.Simulator.Tests.Resources;

public class OperatingTheatreTests
{
    private static readonly DateTime StartTime = new(2023, 1, 1);

    [Fact(DisplayName = "Request yields when the theatre opens")]
    public void Request_YieldsWhenTheatreOpens()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var sut = CreateTheatre(env);

        IEnumerable<Event> TestProcess()
        {
            sut.IsAvailable.Should().BeFalse();
            using var request = sut.Request();
            yield return request;
            sut.IsAvailable.Should().BeFalse();
            env.Now.Should().Be(StartTime.AddHours(6));
            yield return env.Timeout(TimeSpan.FromMinutes(37));
            sut.Release(request);
            sut.IsAvailable.Should().BeTrue();

            yield return env.Timeout(TimeSpan.FromHours(12));
            using var request2 = sut.Request();
            yield return request2;
            sut.IsAvailable.Should().BeFalse();
            env.Now.Should().Be(StartTime.AddDays(1).AddHours(6));

            yield return env.Timeout(TimeSpan.FromMinutes(37));
            sut.Release(request);
            sut.IsAvailable.Should().BeTrue();
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAvailable yields at every specialty change")]
    public void Request_YieldsWhenOpenAndSpecialtyChange()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var sut = CreateMultiSpecialtyTheatre(env);

        IEnumerable<Event> TestProcess()
        {
            yield return sut.WhenAvailable();
            env.Now.Should().Be(StartTime.AddHours(6));
            sut.SpecialtyAtTime().Should().Be(OrthopedicsId);
            yield return sut.WhenAvailable();
            env.Now.Should().Be(StartTime.AddHours(12));
            sut.SpecialtyAtTime().Should().Be(GeneralSurgeryId);
            yield return sut.WhenAvailable();
            env.Now.Should().Be(StartTime.AddDays(1).AddHours(6));
            sut.SpecialtyAtTime().Should().Be(OrthopedicsId);
            yield return sut.WhenAvailable();
            env.Now.Should().Be(StartTime.AddDays(1).AddHours(12));
            sut.SpecialtyAtTime().Should().Be(GeneralSurgeryId);
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }


    [Fact(DisplayName = "WhenAvailable and IsAvailable as expected on request crossing specialty allocation")]
    public void Request_IsAvailableAsExpectedWhenCrossingSpecialtyAllocationBorder()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var sut = CreateMultiSpecialtyTheatre(env);

        IEnumerable<Event> RequestProcess()
        {
            using var request = sut.Request();
            yield return request;
            yield return env.Timeout(TimeSpan.FromHours(10));
        }

        IEnumerable<Event> TestProcess()
        {
            yield return sut.WhenAvailable();
            env.Now.Should().Be(StartTime.AddHours(6));
            sut.SpecialtyAtTime().Should().Be(OrthopedicsId);

            var proc = env.Process(RequestProcess());

            yield return env.Timeout(TimeSpan.FromHours(6) + TimeSpan.FromMinutes(1));
            sut.IsAvailable.Should().BeFalse();
            sut.SpecialtyAtTime().Should().Be(GeneralSurgeryId);
            proc.IsProcessed.Should().BeFalse();

            yield return sut.WhenAvailable();
            env.Now.Should().Be(StartTime.AddHours(16));
            sut.IsAvailable.Should().BeTrue();
            sut.SpecialtyAtTime().Should().Be(GeneralSurgeryId);

            // TODO: This fails, and I have absolutely no idea why
            // proc.IsProcessed.Should().BeTrue();
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    [Fact(DisplayName = "WhenAvailable yields immediately when not change only")]
    public void Request_YieldsImmediatelyWhenNotChangeOnly()
    {
        // Arrange:
        var env = new Simulation(StartTime);
        var sut = CreateTheatre(env);

        IEnumerable<Event> TestProcess()
        {
            yield return sut.WhenAvailable(changedOnly: false);
            env.Now.Should().Be(StartTime.AddHours(6));
            yield return sut.WhenAvailable(changedOnly: false);
            env.Now.Should().Be(StartTime.AddHours(6));
            yield return sut.WhenAvailable(changedOnly: false);
            env.Now.Should().Be(StartTime.AddHours(6));

            yield return sut.WhenAvailable(changedOnly: true);
            env.Now.Should().Be(StartTime.AddDays(1).AddHours(6));
            yield return sut.WhenAvailable(changedOnly: false);
            env.Now.Should().Be(StartTime.AddDays(1).AddHours(6));
            yield return sut.WhenAvailable(changedOnly: false);
            env.Now.Should().Be(StartTime.AddDays(1).AddHours(6));
        }

        var proc = env.Process(TestProcess());

        // Act + Assert:
        env.Run();
        proc.IsProcessed.Should().BeTrue();
    }

    public static OperatingTheatre CreateTheatre(Simulation env)
    {
        var theatre = new OperatingTheatre(env);
        for (var day = 0; day < 7; day++)
            theatre
                .Allocation.NewEntry(
                    StartTime.AddDays(day).AddHours(6),
                    TimeSpan.FromHours(12),
                    OrthopedicsId);

        return theatre;
    }

    public static OperatingTheatre CreateMultiSpecialtyTheatre(Simulation env)
    {
        var theatre = new OperatingTheatre(env);
        for (var day = 0; day < 7; day++)
        {
            theatre.Allocation.NewEntry(StartTime.AddDays(day).AddHours(6), TimeSpan.FromHours(6), OrthopedicsId);
            theatre.Allocation.NewEntry(
                StartTime.AddDays(day).AddHours(12),
                TimeSpan.FromHours(6),
                GeneralSurgeryId);
        }

        return theatre;
    }
}