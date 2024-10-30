using Damona.Simulator.Domain;
using Damona.Simulator.Monitors.Reports;
using static Damona.Simulator.Tests.SpecialtyIds;

namespace Damona.Simulator.Tests.Monitors.Reports;

public class SurgeryReportTests
{
    [Fact(DisplayName = "AddInitial adds to the initial count overall, but does nothing more")]
    public void AddInitial_AddsToInitialButNoPeriods()
    {
        // Arrange:
        var sut = new SurgeryReport();

        // Act:
        for (var i = 0; i < 134; i++)
            sut.AddInitial(OrthopedicsId);

        for (var i = 0; i < 32; i++)
            sut.AddInitial(GastroId);

        for (var i = 0; i < 94; i++)
            sut.AddInitial(EarNoseThroatId);

        // Assert:
        sut.Should()
            .BeEquivalentTo(
                new Dictionary<Specialty, SurgeryReport.Entry>
                {
                    { OrthopedicsId, new SurgeryReport.Entry { Initial = 134 } },
                    { GastroId, new SurgeryReport.Entry { Initial = 32 } },
                    { EarNoseThroatId, new SurgeryReport.Entry { Initial = 94 } },
                });
    }

    [Fact(DisplayName = "AddNew adds to correct month and transfers from previous month")]
    public void AddNew_ConsecutiveMonths()
    {
        // Arrange:
        var sut = new SurgeryReport();

        // Act:
        sut.AddInitial(OrthopedicsId, 137);
        sut.AddInitial(EarNoseThroatId, 34);
        sut.AddNew(OrthopedicsId, new(2023, 1, 1), 24);
        sut.AddNew(EarNoseThroatId, new(2023, 1, 4), 11);
        sut.AddNew(OrthopedicsId, new(2023, 2, 8), 67);
        sut.AddNew(EarNoseThroatId, new(2023, 2, 2), 14);

        // Assert:
        sut.Should()
            .BeEquivalentTo(
                new Dictionary<Specialty, SurgeryReport.Entry>
                {
                    {
                        OrthopedicsId,
                        new SurgeryReport.Entry
                        {
                            Initial = 137, New = 24 + 67,
                            Periods =
                            {
                                new() { Month = new(2023, 1, 1), Initial = 137, New = 24 },
                                new() { Month = new(2023, 2, 1), Initial = 137 + 24, New = 67 }
                            }
                        }
                    },
                    {
                        EarNoseThroatId, new SurgeryReport.Entry
                        {
                            Initial = 34, New = 11 + 14,
                            Periods =
                            {
                                new() { Month = new(2023, 1, 1), Initial = 34, New = 11 },
                                new() { Month = new(2023, 2, 1), Initial = 34 + 11, New = 14 }
                            }
                        }
                    },
                });
    }

    [Fact(DisplayName = "AddNew throws an exception when updated in non-chronological order")]
    public void AddNew_NonChronological()
    {
        // Arrange:
        var sut = new SurgeryReport();

        // Act:
        sut.AddInitial(OrthopedicsId, 137);
        sut.AddNew(OrthopedicsId, new(2023, 1, 1), 24);
        sut.AddNew(OrthopedicsId, new(2023, 2, 8), 67);

        // Assert:
        sut.Invoking(x => x.AddNew(OrthopedicsId, new(2023, 1, 14), 11)).Should().Throw<ArgumentException>();
    }

    [Fact(DisplayName = "AddCompleted adds to correct month and transfers from previous month")]
    public void AddCompleted_ConsecutiveMonths()
    {
        // Arrange:
        var sut = new SurgeryReport();

        // Act:
        sut.AddInitial(OrthopedicsId, 137);
        sut.AddInitial(EarNoseThroatId, 34);
        sut.AddCompleted(OrthopedicsId, new(2023, 1, 1), 24);
        sut.AddCompleted(OrthopedicsId, new(2023, 1, 14), 33);
        sut.AddCompleted(EarNoseThroatId, new(2023, 1, 4), 11);
        sut.AddCompleted(OrthopedicsId, new(2023, 2, 8), 67);
        sut.AddCompleted(EarNoseThroatId, new(2023, 2, 2), 14);

        // Assert:
        sut.Should()
            .BeEquivalentTo(
                new Dictionary<Specialty, SurgeryReport.Entry>
                {
                    {
                        OrthopedicsId,
                        new SurgeryReport.Entry
                        {
                            Initial = 137, Completed = 24 + 33 + 67,
                            Periods =
                            {
                                new() { Month = new(2023, 1, 1), Initial = 137, Completed = 24 + 33 },
                                new() { Month = new(2023, 2, 1), Initial = 137 - 24 - 33, Completed = 67 }
                            }
                        }
                    },
                    {
                        EarNoseThroatId, new SurgeryReport.Entry
                        {
                            Initial = 34, Completed = 11 + 14,
                            Periods =
                            {
                                new() { Month = new(2023, 1, 1), Initial = 34, Completed = 11 },
                                new() { Month = new(2023, 2, 1), Initial = 34 - 11, Completed = 14 }
                            }
                        }
                    },
                });
    }

    [Fact(DisplayName = "AddCompleted throws an exception when updated in non-chronological order")]
    public void AddCompleted_NonChronological()
    {
        // Arrange:
        var sut = new SurgeryReport();

        // Act:
        sut.AddInitial(OrthopedicsId, 137);
        sut.AddCompleted(OrthopedicsId, new(2023, 1, 1), 24);
        sut.AddCompleted(OrthopedicsId, new(2023, 2, 8), 67);

        // Assert:
        sut.Invoking(x => x.AddCompleted(OrthopedicsId, new(2023, 1, 14), 11)).Should().Throw<ArgumentException>();
    }
}