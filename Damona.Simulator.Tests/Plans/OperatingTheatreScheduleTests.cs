using Damona.Simulator.Domain;
using Damona.Simulator.Plans;

namespace Damona.Simulator.Tests.Plans;

public class OperatingTheatreScheduleTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void EntriesAddedInOrder()
    {
        // Arrange:
        var sut = new OperatingTheatreSchedule(DateTime.MinValue);
        var electiveSurgery = _fixture.Create<ElectiveSurgery>();

        // Act
        sut.NewEntry(new(2023, 01, 01, 14, 0, 0), TimeSpan.FromHours(4), electiveSurgery);
        sut.NewEntry(new(2023, 01, 01, 10, 0, 0), TimeSpan.FromHours(1), electiveSurgery);
        sut.NewEntry(new(2023, 01, 01, 11, 0, 0), TimeSpan.FromHours(1), electiveSurgery);

        // Assert:
        sut.Entries.Should()
            .BeEquivalentTo(
                new PlannedSurgery[]
                {
                    new(new(2023, 01, 01, 14, 0, 0), TimeSpan.FromHours(4), electiveSurgery),
                    new(new(2023, 01, 01, 10, 0, 0), TimeSpan.FromHours(1), electiveSurgery),
                    new(new(2023, 01, 01, 11, 0, 0), TimeSpan.FromHours(1), electiveSurgery)
                });
    }

    [Fact]
    public void ThrowsWhenAddingEntriesBeforeNow()
    {
        // Arrange:
        var sut = new OperatingTheatreSchedule(new DateTime(2023, 02, 01, 13, 0, 0));

        // Assert:
        sut.Invoking(
                schedule => schedule.NewEntry(
                    new(2023, 01, 01, 10, 0, 0),
                    TimeSpan.FromHours(1),
                    _fixture.Create<ElectiveSurgery>()))
            .Should()
            .Throw<ArgumentException>();

        sut.Invoking(
                schedule => schedule.NewEntry(
                    new(2023, 02, 01, 12, 59, 59),
                    TimeSpan.FromMinutes(1),
                    _fixture.Create<ElectiveSurgery>()))
            .Should()
            .Throw<ArgumentException>();

        sut.Invoking(
                schedule => schedule.NewEntry(
                    new(2023, 02, 01, 13, 0, 0),
                    TimeSpan.FromMinutes(1),
                    _fixture.Create<ElectiveSurgery>()))
            .Should()
            .NotThrow();
    }
}