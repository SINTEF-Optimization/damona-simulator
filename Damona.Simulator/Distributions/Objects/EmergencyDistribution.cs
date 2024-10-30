using Damona.Simulator.Domain;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Simulator.Distributions.Objects;

/// <summary>
///     A <see cref="IEmergencyDistribution"/> where we sample empirically the time of the emergency, the specialty of
///     the emergency, and the duration of the emergency surgery.
/// </summary>
public class EmergencyDistribution : IEmergencyDistribution
{
    private readonly IRandom _random;
    private readonly Dictionary<Urgency, EmpiricalNonUniform<DistributionElement>> _distributions;

    private readonly Dictionary<Urgency, Dictionary<Specialty, EmpiricalNonUniform<TimeSpan>>>
        _plannedTimeDistributions;

    public EmergencyDistribution(
        IRandom random,
        Dictionary<Urgency, EmpiricalNonUniform<DistributionElement>> distributions,
        Dictionary<Urgency, Dictionary<Specialty, EmpiricalNonUniform<TimeSpan>>> plannedTimeDistributions)
    {
        _random = random;
        _distributions = distributions;
        _plannedTimeDistributions = plannedTimeDistributions;
    }

    public Emergency Sample(DateTime sampleTime, Urgency urgency)
    {
        // We first sample specialty of the surgery, whether it occurs on a weekday or the weekend, and which hour
        // of the day the surgery happens on.
        var sample = _distributions[urgency].Sample(_random);

        // Then sample the duration of the surgery based on urgency and specialty
        var duration = _plannedTimeDistributions[urgency][sample.Specialty].Sample(_random);

        return new()
        {
            EmergencyDate = SampleEmergencyDate(sampleTime, sample),
            Urgency = urgency,
            Specialty = sample.Specialty,
            EstimatedDuration = duration
        };
    }

    /// <summary>
    ///     Sample the date and time of the emergency based on a sampled <see cref="DistributionElement"/>.
    /// </summary>
    private DateTime SampleEmergencyDate(DateTime sampleTime, DistributionElement sample)
    {
        var date = sampleTime.MondayCurrentWeek().Date;

        date += sample.IsWeekend
            ? TimeSpan.FromDays(5 + _random.Next(2))
            : TimeSpan.FromDays(_random.Next(5));

        date += TimeSpan.FromHours(sample.Hour);

        if (date < sampleTime)
            date += TimeSpan.FromDays(7);

        return date;
    }

    public record DistributionElement(Specialty Specialty, bool IsWeekend, int Hour);
}