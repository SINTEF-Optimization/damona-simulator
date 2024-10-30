using Damona.Simulator.Distributions.Objects;
using Damona.Simulator.Domain;
using SimSharp;

namespace Damona.Simulator.Distributions;

/// <summary>
///     An emergency distribution that is normal distributed in number of emergencies, and these are furthermore
///     distributed uniformly over time (weekly)
/// </summary>
public class NewEmergenciesNormalUniformDistribution : INewEmergenciesDistribution
{
    private DateTime _time;
    private readonly IRandom _random;

    private readonly IEmergencyDistribution _emergencyDistribution;

    /// <summary>
    ///     Distribution for the number of urgency 1 emergencies
    /// </summary>
    private readonly Normal _urgency1Distribution;

    /// <summary>
    ///     Distribution for the number of urgency 2 emergencies
    /// </summary>
    private readonly Normal _urgency2Distribution;

    /// <summary>
    ///     Distribution for the number of urgency 2 emergencies
    /// </summary>
    private readonly Normal _urgency3Distribution;

    private Queue<Emergency> _emegencyQueue = new();

    public NewEmergenciesNormalUniformDistribution(
        DateTime startTime,
        IRandom random,
        IEmergencyDistribution emergencyDistribution,
        DistributionParameters urgency1Parameters,
        DistributionParameters urgency2Parameters,
        DistributionParameters urgency3Parameters)
    {
        _time = startTime;
        _random = random;
        _emergencyDistribution = emergencyDistribution;
        _urgency1Distribution = new(urgency1Parameters.Mean, urgency1Parameters.StdDev);
        _urgency2Distribution = new(urgency2Parameters.Mean, urgency2Parameters.StdDev);
        _urgency3Distribution = new(urgency3Parameters.Mean, urgency3Parameters.StdDev);
    }

    public Emergency? Next()
    {
        if (_emegencyQueue.Count == 0)
            PopulateEmergencies();

        return _emegencyQueue.Dequeue();
    }

    /// <summary>
    ///     Fill the queue of emergencies with the upcoming emergencies for the next week.
    /// </summary>
    private void PopulateEmergencies()
    {
        var newEmergencies = new List<Emergency>();

        var urgency1Emergencies = Math.Round(_urgency1Distribution.Sample(_random));
        for (var i = 0; i < urgency1Emergencies; i++)
            newEmergencies.Add(_emergencyDistribution.Sample(_time, (Urgency)1));

        var urgency2Emergencies = Math.Round(_urgency2Distribution.Sample(_random));
        for (var i = 0; i < urgency2Emergencies; i++)
            newEmergencies.Add(_emergencyDistribution.Sample(_time, (Urgency)2));

        var urgency3Emergencies = Math.Round(_urgency3Distribution.Sample(_random));
        for (var i = 0; i < urgency3Emergencies; i++)
            newEmergencies.Add(_emergencyDistribution.Sample(_time, (Urgency)3));

        foreach (var emergency in newEmergencies.OrderBy(x => x.EmergencyDate))
            _emegencyQueue.Enqueue(emergency);

        _time += TimeSpan.FromDays(7);
    }

    /// <summary>
    ///     Parameters for a normal distribution.
    /// </summary>
    public record DistributionParameters(double Mean, double StdDev);
}