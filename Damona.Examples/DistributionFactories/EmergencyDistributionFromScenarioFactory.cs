using Damona.Schemas;
using Damona.Schemas.AutoMapper;
using Damona.Schemas.Distributions;
using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Examples.DistributionFactories;

/// <summary>
///     Factory for a <see cref="EmergencyDistribution"/> that reads the empirical distributions from file
///     based on the <see cref="EmergencyFrequencies"/> and <see cref="PlannedSurgeryTimeFrequencies"/> formats.
/// </summary>
public class EmergencyDistributionFromScenarioFactory(
    SpecialtyMap map,
    Scenario scenario)
    : IEmergencyDistributionFactory
{
    public IEmergencyDistribution Create(IRandom random)
    {
        var distributions = new Dictionary<int, EmpiricalNonUniform<EmergencyDistribution.DistributionElement>>();

        foreach (var item in scenario.Distributions?.Emergencies ?? [])
        {
            var values = new List<EmergencyDistribution.DistributionElement>();
            var weights = new List<double>();

            foreach (var (specialty, freqs) in item.Frequencies.Weekday)
            {
                for (int i = 0; i < freqs.Length; i++)
                {
                    if (freqs[i] == 0)
                        continue;

                    values.Add(new(map.GetOrAdd(specialty), false, i));
                    weights.Add(freqs[i]);
                }
            }

            distributions[item.Urgency] = new(values, weights);
        }

        var plannedTimeDistributions = new Dictionary<Urgency, Dictionary<Specialty, EmpiricalNonUniform<TimeSpan>>>();

        foreach (var item in scenario.Distributions?.EmergencySurgeryTime ?? [])
        {
            plannedTimeDistributions[item.Urgency] = new();

            foreach (var (specialty, histogramCounts) in item.Frequencies)
            {
                plannedTimeDistributions[item.Urgency][map.GetOrAdd(specialty)] = new(
                    histogramCounts.Values.Select(TimeSpan.FromMinutes),
                    histogramCounts.Counts);
            }
        }

        return new EmergencyDistribution(random, distributions, plannedTimeDistributions);
    }
}