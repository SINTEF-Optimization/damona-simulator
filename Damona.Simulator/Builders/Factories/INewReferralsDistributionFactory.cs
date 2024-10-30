using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory for an <see cref="INewReferralsDistribution"/>
/// </summary>
public interface INewReferralsDistributionFactory
{
    INewReferralsDistribution Create(IRandom random, IElectiveSurgeryDistribution distribution);
}