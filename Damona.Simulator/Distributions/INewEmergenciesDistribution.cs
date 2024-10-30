using Damona.Simulator.Domain;

namespace Damona.Simulator.Distributions;

/// <summary>
///     Distribution of <see cref="Emergency"/>. Used by consecutively calling <see cref="Next"/> which gives the next
///     emergency that must be handled.
/// </summary>
public interface INewEmergenciesDistribution
{
    /// <summary>
    ///     Retrieve the next <see cref="Emergency"/> given by the distribution.
    /// </summary>
    Emergency? Next();
}