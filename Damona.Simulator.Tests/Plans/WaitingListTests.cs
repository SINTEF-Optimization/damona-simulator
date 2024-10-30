using Damona.Simulator.Domain;
using Damona.Simulator.Plans;
using static Damona.Simulator.Tests.SpecialtyIds;

namespace Damona.Simulator.Tests.Plans;

public class WaitingListTests
{
    [Fact(DisplayName = "Next returns only element when it fits the operation window window")]
    public void Next_OneSurgeryInList_ReturnsExpected()
    {
        // Arrange:
        var surgeries = new[] { CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(30)) };
        var sut = new WaitingList(surgeries);

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(29)).Should().BeNull();
        sut.Next(OdontologyId, TimeSpan.FromMinutes(60)).Should().BeNull();
    }

    [Fact(DisplayName = "Next returns later surgery of different specialty when window has that")]
    public void Next_MultipleSpecialties_ReturnsDifferentSpecialty()
    {
        // Arrange:
        var surgeries = new[]
        {
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateSurgery(OdontologyId, TimeSpan.FromMinutes(20)),
            CreateSurgery(OdontologyId, TimeSpan.FromMinutes(20))
        };
        var sut = new WaitingList(surgeries);

        // Act + Assert:
        sut.Next(OdontologyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[2]);
        sut.Next(OdontologyId, TimeSpan.FromMinutes(10)).Should().BeNull();
    }

    [Fact(DisplayName = "Next returns later surgery of different duration when window has space for that")]
    public void Next_MultipleTypes_ReturnsLaterSurgery()
    {
        // Arrange:
        var surgeries = new[]
        {
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(60)),
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(60)),
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(30)),
        };
        var sut = new WaitingList(surgeries);

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(40)).Should().Be(surgeries[2]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(20)).Should().BeNull();
    }

    [Fact(DisplayName = "Next returns later surgery of different specialty and duration when window has that")]
    public void Next_MultipleSpecialtiesAndTypes_ReturnsDifferentSpecialty()
    {
        // Arrange:
        var surgeries = new[]
        {
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(60)),
            CreateSurgery(OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateSurgery(OdontologyId, TimeSpan.FromMinutes(40)),
            CreateSurgery(OdontologyId, TimeSpan.FromMinutes(20))
        };
        var sut = new WaitingList(surgeries);

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(40)).Should().Be(surgeries[1]);
        sut.Next(OdontologyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[2]);
        sut.Next(OdontologyId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[3]);
    }

    private static ElectiveSurgery CreateSurgery(Specialty specialty, TimeSpan duration)
    {
        return new()
        {
            Specialty = specialty,
            EstimatedDuration = duration
        };
    }
}