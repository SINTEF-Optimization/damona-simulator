using Damona.Simulator.Domain;

namespace Damona.Simulator.Plans;

public interface IWaitingList
{
    void AddElectiveSurgery(ElectiveSurgery electiveSurgery);

    void AddElectiveSurgeries(IEnumerable<ElectiveSurgery> electiveSurgeries);

    bool RemoveElectiveSurgery(ElectiveSurgery electiveSurgery);

    IEnumerable<ElectiveSurgery> GetAll();

    /// <summary>
    ///     Get the next <see cref="ElectiveSurgery"/> that fits a time slot specified by a <see cref="Specialty"/>.
    /// </summary>
    /// <param name="specialty">The speciality of the slot</param>
    /// <param name="availableTime">The time available in the slot</param>
    /// <returns>The next elective surgery in the queue, <c>null</c> if no surgeries fit the slot</returns>
    ElectiveSurgery? Next(Specialty specialty, TimeSpan availableTime);
}