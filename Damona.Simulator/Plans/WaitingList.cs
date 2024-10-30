using Damona.Simulator.Domain;

namespace Damona.Simulator.Plans;

public class WaitingList : IWaitingList
{
    public event EventHandler? Updated;

    private readonly Dictionary<Specialty, List<ElectiveSurgery>> _waitingList = new();

    public WaitingList()
    {
    }

    public WaitingList(IEnumerable<ElectiveSurgery> surgeries)
    {
        foreach (var surgery in surgeries)
            AddToWaitingList(surgery);
    }

    public void AddElectiveSurgery(ElectiveSurgery electiveSurgery)
    {
        AddToWaitingList(electiveSurgery);
        OnUpdated();
    }

    public void AddElectiveSurgeries(IEnumerable<ElectiveSurgery> electiveSurgeries)
    {
        foreach (var electiveSurgery in electiveSurgeries)
            AddToWaitingList(electiveSurgery);

        OnUpdated();
    }

    public bool RemoveElectiveSurgery(ElectiveSurgery electiveSurgery)
    {
        if (!_waitingList.ContainsKey(electiveSurgery.Specialty))
            return false;

        return _waitingList[electiveSurgery.Specialty].Remove(electiveSurgery);
    }

    public IEnumerable<ElectiveSurgery> GetAll() => _waitingList.SelectMany(kvp => kvp.Value);

    /// <summary>
    ///     Get the next <see cref="ElectiveSurgery"/> that fits a time slot specified by a <see cref="Specialty"/>.
    /// </summary>
    /// <param name="specialty">The speciality of the slot</param>
    /// <param name="availableTime">The time available in the slot</param>
    /// <returns>The next elective surgery in the queue, <c>null</c> if no surgeries fit the slot</returns>
    public ElectiveSurgery? Next(Specialty specialty, TimeSpan availableTime)
    {
        var queue = _waitingList.GetValueOrDefault(specialty);
        if (queue is null || queue.Count == 0)
            return null;

        // PERF: Need to make sure we don't search through the entire queue when there is only 10 minutes available
        foreach (var surgery in queue)
        {
            if (surgery.EstimatedDuration <= availableTime)
                return surgery;
        }

        return null;
    }

    private void OnUpdated()
    {
        Updated?.Invoke(this, EventArgs.Empty);
    }

    private void AddToWaitingList(ElectiveSurgery electiveSurgery)
    {
        if (!_waitingList.ContainsKey(electiveSurgery.Specialty))
            _waitingList.Add(electiveSurgery.Specialty, new());

        _waitingList[electiveSurgery.Specialty].Add(electiveSurgery);
    }
}