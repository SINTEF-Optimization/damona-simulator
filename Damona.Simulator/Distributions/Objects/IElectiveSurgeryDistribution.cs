using Damona.Simulator.Domain;

namespace Damona.Simulator.Distributions.Objects;

/// <summary>
///     A distribution of <see cref="ElectiveSurgery"/>.
/// </summary>
public interface IElectiveSurgeryDistribution
{
    /// <summary>
    ///     Sample the distribution for an <see cref="ElectiveSurgery"/>.
    /// </summary>
    /// <param name="referralDate">The referral date for the distribution.</param>
    /// <param name="specialty">The <see cref="Specialty"/> we are sampling a surgery for.</param>
    ElectiveSurgery Sample(DateTime referralDate, Specialty specialty);
}