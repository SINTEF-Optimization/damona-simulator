using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory for an <see cref="IEmergencyDistribution"/>
/// </summary>
public interface IEmergencyDistributionFactory
{
    IEmergencyDistribution Create(IRandom random);
}