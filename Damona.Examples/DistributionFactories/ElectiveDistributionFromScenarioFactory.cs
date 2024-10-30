using Damona.Examples.Distributions.Objects;
using Damona.Schemas;
using Damona.Schemas.AutoMapper;
using Damona.Schemas.Distributions;
using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Examples.DistributionFactories;

/// <summary>
///     Factory for a <see cref="IElectiveSurgeryDistribution"/> that reads the empirical distributions from file
///     based on the <see cref="PlannedSurgeryTimeFrequencies"/> format.
/// </summary>
public class ElectiveDistributionFromScenarioFactory(SpecialtyMap map, Scenario scenario)
    : IElectiveSurgeryDistributionFactory
{
    private readonly SpecialtyMap _map = map;
    private readonly Scenario _scenario = scenario;

    public IElectiveSurgeryDistribution Create(IRandom random)
    {
        var frequencies = _scenario.Distributions?.ElectiveSurgeryTime;

        if (frequencies is null)
            return new DummyElectiveSurgeryDistribution();

        var plannedTimeDistributions = new Dictionary<Specialty, Distribution<TimeSpan>>();

        foreach (var (specialty, histogramCounts) in frequencies)
        {
            plannedTimeDistributions[_map.GetOrAdd(specialty)] = new EmpiricalNonUniform<TimeSpan>(
                histogramCounts.Values.Select(TimeSpan.FromMinutes),
                histogramCounts.Counts);
        }

        return new ElectiveSurgeryDistribution(random, plannedTimeDistributions);
    }
}