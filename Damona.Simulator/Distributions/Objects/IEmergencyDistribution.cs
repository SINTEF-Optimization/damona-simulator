using Damona.Simulator.Domain;

namespace Damona.Simulator.Distributions.Objects;

/// <summary>
///     Distribution of <see cref="Emergency"/>.
/// </summary>
public interface IEmergencyDistribution
{
    /// <summary>
    ///     Sample the distribution for an <see cref="Emergency"/>.
    /// </summary>
    /// <param name="sampleTime">The simulated time the sampling is made.</param>
    /// <param name="urgency">The <see cref="Urgency"/> we are sampling from.</param>
    Emergency Sample(DateTime sampleTime, Urgency urgency);
}