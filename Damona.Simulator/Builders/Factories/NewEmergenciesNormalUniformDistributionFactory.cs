using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using SimSharp;
using DistributionParameters =
    Damona.Simulator.Distributions.NewEmergenciesNormalUniformDistribution.DistributionParameters;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory for a <see cref="NewEmergenciesNormalUniformDistribution"/>.
/// </summary>
public class NewEmergenciesNormalUniformDistributionFactory : INewEmergenciesDistributionFactory
{
    private DateTime _startTime;

    /// <summary>
    ///     
    /// </summary>
    public DistributionParameters Urgency1Parameters { get; set; } = new(10.8, 4.1);

    public DistributionParameters Urgency2Parameters { get; set; } = new(17, 4.6);
    public DistributionParameters Urgency3Parameters { get; set; } = new(30, 7);

    public NewEmergenciesNormalUniformDistributionFactory(DateTime startTime)
    {
        _startTime = startTime;
    }

    public INewEmergenciesDistribution Create(IRandom random, IEmergencyDistribution distribution)
    {
        return new NewEmergenciesNormalUniformDistribution(
            _startTime,
            random,
            distribution,
            Urgency1Parameters,
            Urgency2Parameters,
            Urgency3Parameters);
    }
}