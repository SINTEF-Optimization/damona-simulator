using Damona.Simulator.Domain;
using SimSharp;

namespace Damona.Simulator.Distributions.Objects;

/// <summary>
///     A <see cref="IElectiveSurgeryDistribution"/> where there are separate surgery duration distributions for every
///     <see cref="Specialty"/>.
/// </summary>
public class ElectiveSurgeryDistribution : IElectiveSurgeryDistribution
{
    private IRandom _random;
    private Dictionary<Specialty, Distribution<TimeSpan>> _durationDistributions;

    public ElectiveSurgeryDistribution(
        IRandom random,
        Dictionary<Specialty, Distribution<TimeSpan>> durationDistributions)
    {
        _random = random;
        _durationDistributions = durationDistributions;
    }

    /// <summary>
    ///     Sample the distribution for an <see cref="ElectiveSurgery"/>. Only the estimated duration of the surgery is
    ///     sampled, the rest is specified by the request.
    /// </summary>
    /// <remarks>
    ///     If no distribution is provided for the specialty requested it defaults to 20 minutes.
    /// </remarks>
    public ElectiveSurgery Sample(DateTime referralDate, Specialty specialty)
    {
        var duration = _durationDistributions.GetValueOrDefault(specialty)?.Sample(_random) ?? TimeSpan.FromMinutes(20);

        return new()
        {
            ReferralDate = referralDate,
            Specialty = specialty,
            EstimatedDuration = duration
        };
    }
}