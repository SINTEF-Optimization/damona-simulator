using Damona.Schemas.AutoMapper;
using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using Damona.Simulator.Domain;

namespace Damona.Examples.Distributions;

/// <summary>
///     "Distribution" based on the historic number of new elective surgeries in 2022.
/// </summary>
public class NewReferralsFrom2022 : INewReferralsDistribution
{
    private readonly IElectiveSurgeryDistribution _surgeryDistribution;
    private readonly Dictionary<Specialty, int[]> _surgeriesByMonth;

    public IEnumerable<ElectiveSurgery> NewSurgeriesInMonth(DateTime atTime)
    {
        var result = new List<ElectiveSurgery>();
        var monthNumber = atTime.Month;

        foreach (var specialty in _surgeriesByMonth.Keys)
        {
            var numberOfSurgeries = _surgeriesByMonth[specialty][monthNumber - 1];

            for (var i = 0; i < numberOfSurgeries; i++)
                result.Add(_surgeryDistribution.Sample(atTime, specialty));
        }

        return result;
    }

    // ReSharper disable once ConvertToPrimaryConstructor
    public NewReferralsFrom2022(IElectiveSurgeryDistribution surgeryDistribution, SpecialtyMap specialtyMap)
    {
        _surgeryDistribution = surgeryDistribution;
        _surgeriesByMonth = new()
        {
            { specialtyMap.GetOrAdd("specialty-1"), [90, 103, 98, 112, 71, 123, 85, 85, 101, 117, 137, 89] },
            { specialtyMap.GetOrAdd("specialty-2"), [278, 265, 328, 246, 271, 302, 169, 250, 332, 315, 374, 211] },
            { specialtyMap.GetOrAdd("specialty-3"), [246, 282, 310, 257, 279, 235, 191, 241, 356, 319, 360, 25] },
            { specialtyMap.GetOrAdd("specialty-4"), [172, 188, 201, 143, 206, 183, 118, 138, 195, 194, 216, 164] },
            { specialtyMap.GetOrAdd("specialty-5"), [56, 55, 63, 63, 42, 68, 67, 77, 60, 69, 60, 64] },
            { specialtyMap.GetOrAdd("specialty-6"), [89, 104, 108, 96, 130, 115, 85, 67, 107, 113, 136, 86] },
            { specialtyMap.GetOrAdd("specialty-7"), [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0] },
            { specialtyMap.GetOrAdd("specialty-8"), [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0] },
            { specialtyMap.GetOrAdd("specialty-9"), [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0] },
        };
    }
}