using Damona.Simulator.Domain;

namespace Damona.Simulator.Distributions;

/// <summary>
///     Dummy <see cref="INewEmergenciesDistribution"/> which has no emergencies.
/// </summary>
public class NoNewEmergenciesesDistribution : INewEmergenciesDistribution
{
    public Emergency? Next()
    {
        return null;
    }
}