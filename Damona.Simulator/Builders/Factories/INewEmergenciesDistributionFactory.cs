using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory for an <see cref="INewEmergenciesDistribution"/>
/// </summary>
public interface INewEmergenciesDistributionFactory
{
    INewEmergenciesDistribution Create(IRandom random, IEmergencyDistribution distribution);
}