using Damona.Simulator.Domain;

namespace Damona.Simulator.Plans;

public interface IEmergencyList
{
    bool IsEmpty { get; }

    IEnumerable<Emergency> GetAll();

    /// <summary>
    ///     Add an emergency to the emergency list.
    /// </summary>
    void AddEmergency(Emergency emergency);

    /// <summary>
    ///     Remove an emergency from the emergency list
    /// </summary>
    bool RemoveEmergency(Emergency emergency);

    /// <summary>
    ///     Get the next <see cref="Emergency"/> that fits a time slot specified by a <see cref="Specialty"/>.
    /// </summary>
    /// <param name="specialty">The speciality of the slot</param>
    /// <param name="availableTime">The time available in the slot</param>
    /// <returns>The next emergency in the queue, <c>null</c> if no emergencies fit the slot</returns>
    Emergency? Next(Specialty specialty, TimeSpan availableTime);
}