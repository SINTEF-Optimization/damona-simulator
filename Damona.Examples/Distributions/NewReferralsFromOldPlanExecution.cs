using Damona.Schemas.AutoMapper;
using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using Damona.Simulator.Domain;
using SimSharp;

namespace Damona.Examples.Distributions;

/// <summary>
///     Distribution of elective surgeries based on the surgery numbers we got from running the simulation with
///     <see cref="BasePlanScenarioFactory"/>.
/// </summary>
public class NewReferralsFromOldPlanExecution : INewReferralsDistribution
{
    private IRandom _random;
    private readonly Dictionary<Specialty, Distribution<double>> _distributions;
    private readonly IElectiveSurgeryDistribution _surgeryDistribution;

    public IEnumerable<ElectiveSurgery> NewSurgeriesInMonth(DateTime atTime)
    {
        var result = new List<ElectiveSurgery>();

        foreach (var (specialty, dist) in _distributions)
        {
            var numberOfSurgeries = Math.Round(dist.Sample(_random));

            for (var i = 0; i < numberOfSurgeries; i++)
                result.Add(_surgeryDistribution.Sample(atTime, specialty));
        }

        return result;
    }

    // ReSharper disable once ConvertToPrimaryConstructor
    public NewReferralsFromOldPlanExecution(
        IRandom random,
        IElectiveSurgeryDistribution surgeryDistribution,
        SpecialtyMap specialtyMap)
    {
        _random = random;
        _surgeryDistribution = surgeryDistribution;
        _distributions = new()
            {
                { specialtyMap.GetOrAdd("specialty-1"), new Normal(49.6, 5.0) },
                { specialtyMap.GetOrAdd("specialty-2"), new Normal(69.8, 7.0) },
                { specialtyMap.GetOrAdd("specialty-3"), new Normal(138.5, 13.9) },
                { specialtyMap.GetOrAdd("specialty-4"), new Normal(21.6, 2.2) },
                { specialtyMap.GetOrAdd("specialty-5"), new Normal(17.3, 1.7) },
                { specialtyMap.GetOrAdd("specialty-6"), new Normal(53.9, 5.4) },
                { specialtyMap.GetOrAdd("specialty-7"), new Normal(30.3, 3.0) },
                { specialtyMap.GetOrAdd("specialty-8"), new Normal(78, 7.8) },
                { specialtyMap.GetOrAdd("specialty-9"), new Normal(6.5, 0.65) }
            }
            ;
    }
}