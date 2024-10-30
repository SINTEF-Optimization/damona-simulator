using Damona.Simulator.Domain;

namespace Damona.Simulator.Distributions;

/// <summary>
///     Distribution of <see cref="ElectiveSurgery"/>.
///     TODO: Weekly distributions
/// </summary>
public interface INewReferralsDistribution
{
    /// <summary>
    ///     Get the new elective surgeries for a month.
    /// </summary>
    /// <param name="atTime">The time which will both be the month to query as well as be the referral time.</param>
    /// <returns>A collection of new elective surgeries in a month.</returns>
    IEnumerable<ElectiveSurgery> NewSurgeriesInMonth(DateTime atTime);
}