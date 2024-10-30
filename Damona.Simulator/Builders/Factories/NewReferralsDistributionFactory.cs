using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using SimSharp;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory method that creates a <see cref="INewReferralsDistribution"/> from a function object.
/// </summary>
public class NewReferralsDistributionFactory : INewReferralsDistributionFactory
{
    private Func<IRandom, IElectiveSurgeryDistribution, INewReferralsDistribution> _factory;

    public NewReferralsDistributionFactory(
        Func<IRandom, IElectiveSurgeryDistribution, INewReferralsDistribution> factory)
    {
        _factory = factory;
    }

    public INewReferralsDistribution Create(IRandom random, IElectiveSurgeryDistribution distribution)
    {
        return _factory(random, distribution);
    }
}