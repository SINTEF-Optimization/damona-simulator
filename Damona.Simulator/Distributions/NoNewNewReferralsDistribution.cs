using Damona.Simulator.Domain;

namespace Damona.Simulator.Distributions;

/// <summary>
///     Dummy <see cref="INewReferralsDistribution"/> which has no new referrals.
/// </summary>
public class NoNewNewReferralsDistribution : INewReferralsDistribution
{
    public IEnumerable<ElectiveSurgery> NewSurgeriesInMonth(DateTime atTime)
    {
        return Enumerable.Empty<ElectiveSurgery>();
    }
}