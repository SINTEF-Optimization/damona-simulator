using Damona.Simulator.Domain;
using Damona.Simulator.Plans;
using static Damona.Simulator.Constants;
using static Damona.Simulator.Tests.SpecialtyIds;

namespace Damona.Simulator.Tests.Plans;

public class EmergencyListTests
{
    [Fact(
        DisplayName =
            "Next returns only element when it fits the operation window and window has a specific specialty")]
    public void Next_OneSurgeryInList_ReturnsEmergencyWhenCorrectSpecialty()
    {
        // Arrange:
        var surgeries = new[]
            { CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)) };
        var sut = new EmergencyList(surgeries);

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(29)).Should().BeNull();
        sut.Next(OdontologyId, TimeSpan.FromMinutes(60)).Should().BeNull();
    }

    [Fact(DisplayName = "Next returns only element when it fits the emergency emergency operation window")]
    public void Next_OneSurgeryInList_ReturnsSurgeryWhenEmergencySpecialtyAndTimeWindow()
    {
        // Arrange:
        var surgeries = new[]
            { CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)) };
        var sut = new EmergencyList(surgeries);

        // Act + Assert:
        sut.Next(EmergencyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[0]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(29)).Should().BeNull();
    }

    [Fact(DisplayName = "Next returns later surgery of different specialty when window has that")]
    public void Next_MultipleSpecialties_ReturnsDifferentSpecialty()
    {
        #region Arrange

        var emergencies = new[]
        {
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateEmergency((Urgency)1, OdontologyId, TimeSpan.FromMinutes(20)),
            CreateEmergency((Urgency)1, OdontologyId, TimeSpan.FromMinutes(20))
        };

        var sut = new EmergencyList(emergencies);

        #endregion

        // Act + Assert:
        sut.Next(OdontologyId, TimeSpan.FromMinutes(60)).Should().Be(emergencies[2]);
        sut.Next(OdontologyId, TimeSpan.FromMinutes(10)).Should().BeNull();
    }

    [Fact(DisplayName = "Next returns higher urgency surgery before lower urgency when same specialty")]
    public void Next_MultipleUrgenciesSingleSpecialty_ReturnsHigherUrgencyFirst()
    {
        // Arrange:
        var surgeries = new[]
        {
            CreateEmergency((Urgency)3, OrthopedicsId, TimeSpan.FromMinutes(10)),
            CreateEmergency((Urgency)2, OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(60))
        };
        var sut = new EmergencyList(surgeries);

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[2]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[1]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(20)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(5)).Should().BeNull();

        sut.Next(EmergencyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[2]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[1]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(20)).Should().Be(surgeries[0]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(5)).Should().BeNull();
    }

    [Fact(
        DisplayName = "Next returns the first in queue when emergency window",
        Skip = "This doesn't work at the moment because they are only ordered by specialty")]
    public void Next_MultipleSpecialties_ReturnsFirstInQueueWhenEmergencyWindow()
    {
        // Arrange:
        var surgeries = new[]
        {
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateEmergency((Urgency)1, OdontologyId, TimeSpan.FromMinutes(20))
        };
        var sut = new EmergencyList(surgeries);

        // Act + Assert:
        sut.Next(EmergencyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(20)).Should().Be(surgeries[1]);
    }

    [Fact(
        DisplayName =
            "Next returns later surgery of different surgery type when window has space for that and times differ",
        Skip = "Not implemented")]
    public void Next_MultipleTypes_ReturnsLaterSurgery()
    {
        // Arrange:
        var surgeries = new[]
        {
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(60)),
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(60)),
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)),
        };
        var sut = new EmergencyList(surgeries);

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(40)).Should().Be(surgeries[2]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(20)).Should().BeNull();

        sut.Next(EmergencyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(40)).Should().Be(surgeries[2]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(20)).Should().BeNull();
    }

    [Fact(
        DisplayName = "Next returns later surgery of different specialty and type when window has that",
        Skip = "Not implemented")]
    public void Next_MultipleSpecialtiesAndTypes_ReturnsDifferentSpecialty()
    {
        #region Arrange

        var surgeries = new[]
        {
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(60)),
            CreateEmergency((Urgency)1, OrthopedicsId, TimeSpan.FromMinutes(30)),
            CreateEmergency((Urgency)1, OdontologyId, TimeSpan.FromMinutes(40)),
            CreateEmergency((Urgency)1, OdontologyId, TimeSpan.FromMinutes(20))
        };
        var sut = new EmergencyList(surgeries);

        #endregion

        // Act + Assert:
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(OrthopedicsId, TimeSpan.FromMinutes(40)).Should().Be(surgeries[1]);
        sut.Next(OdontologyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[2]);
        sut.Next(OdontologyId, TimeSpan.FromMinutes(30)).Should().Be(surgeries[3]);

        sut.Next(EmergencyId, TimeSpan.FromMinutes(60)).Should().Be(surgeries[0]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(40)).Should().Be(surgeries[1]);
        sut.Next(EmergencyId, TimeSpan.FromMinutes(20)).Should().Be(surgeries[3]);
    }

    private static Emergency CreateEmergency(
        Urgency urgency,
        Specialty specialty,
        TimeSpan duration)
    {
        return new()
        {
            Urgency = urgency,
            Specialty = specialty,
            EstimatedDuration = duration,
            EmergencyDate = DateTime.Parse("2023-01-01T12:00:00")
        };
    }
}