using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory for an <see cref="IElectiveSurgeryDistribution"/>
/// </summary>
public interface IElectiveSurgeryDistributionFactory
{
    IElectiveSurgeryDistribution Create(IRandom random);
}