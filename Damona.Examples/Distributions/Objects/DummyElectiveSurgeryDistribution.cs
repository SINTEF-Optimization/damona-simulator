using Damona.Schemas.Distributions;
using Damona.Simulator.Distributions.Objects;
using Damona.Simulator.Domain;

namespace Damona.Examples.Distributions.Objects;

/// <summary>
///     A dummy <see cref="IElectiveSurgeryDistribution"/> for when there is no distribution data in
///     <see cref="SurgeryDistributions"/>.
/// </summary>
public class DummyElectiveSurgeryDistribution : IElectiveSurgeryDistribution
{
    public ElectiveSurgery Sample(DateTime referralDate, int specialty) =>
        new()
        {
            ReferralDate = referralDate,
            Specialty = specialty,
            EstimatedDuration = TimeSpan.FromMinutes(30)
        };
}