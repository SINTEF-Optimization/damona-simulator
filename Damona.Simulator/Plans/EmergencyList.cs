using Damona.Simulator.Domain;
using static Damona.Simulator.Constants;

namespace Damona.Simulator.Plans;

public class EmergencyList : IEmergencyList
{
    public event EventHandler? NewEmergency;

    private readonly SortedDictionary<Urgency, List<Emergency>> _emergencies = new();

    public bool IsEmpty => _emergencies.All(kvp => kvp.Value.Count == 0);

    public EmergencyList()
    {
    }

    public EmergencyList(IEnumerable<Emergency> emergencies)
    {
        foreach (var emergency in emergencies)
            AddEmergencyToQueue(emergency);
    }

    public Emergency? Next(Specialty specialty, TimeSpan availableTime)
    {
        if (IsEmpty)
            return null;

        if (specialty == EmergencyId)
            return Next(availableTime);

        foreach (var urgency in _emergencies.Keys)
        {
            foreach (var emergency in _emergencies[urgency])
            {
                if (emergency.Specialty != specialty)
                    continue;

                if (emergency.EstimatedDuration > availableTime)
                    continue;

                return emergency;
            }
        }

        return null;
    }

    /// <summary>
    ///     Get the next <see cref="Emergency"/> that fits an open time slot. This assumes the slot can be used by any
    ///     emergency.
    /// </summary>
    /// <param name="availableTime">The time available in the slot</param>
    /// <returns>The next emergency in the queue, <c>null</c> if no emergencies fit the slot</returns>
    private Emergency? Next(TimeSpan availableTime)
    {
        foreach (var urgency in _emergencies.Keys)
        {
            foreach (var emergency in _emergencies[urgency])
            {
                if (emergency.EstimatedDuration > availableTime)
                    continue;

                return emergency;
            }
        }

        return null;
    }

    public IEnumerable<Emergency> GetAll() => _emergencies.SelectMany(kvp => kvp.Value);

    public void AddEmergency(Emergency emergency)
    {
        AddEmergencyToQueue(emergency);
        OnNewEmergency();
    }

    public bool RemoveEmergency(Emergency emergency)
    {
        if (!_emergencies.ContainsKey(emergency.Urgency))
            return false;

        return _emergencies[emergency.Urgency].Remove(emergency);
    }

    private void AddEmergencyToQueue(Emergency emergency)
    {
        if (!_emergencies.ContainsKey(emergency.Urgency))
            _emergencies.Add(emergency.Urgency, []);

        _emergencies[emergency.Urgency].Add(emergency);
    }

    private void OnNewEmergency()
    {
        NewEmergency?.Invoke(this, EventArgs.Empty);
    }
}