using Damona.Simulator.Domain;
using Damona.Simulator.Processes.Operations;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Simulator.Resources;

/// <summary>
///     An operating theatre resource. A theatre has a schedule, which is a set of specialty allocations.
///
///      Requests for the theatre is handled FIFO, with no priority for different fields. This means that it is the
///     <see cref="IOperationsProcess"/> that has to handle this prioritization.
/// </summary>
public sealed class OperatingTheatre
{
    private bool _inUse = false;

    private bool _isOpen = false;

    public Schedule<Specialty> Allocation { get; } = new();

    /// <summary>
    ///     Whether the theatre is currently available.
    /// </summary>
    public bool IsAvailable => !_inUse && _isOpen;

    private Simulation Environment { get; }

    private LinkedList<Request> RequestQueue { get; } = new();

    private Queue<Release> ReleaseQueue { get; } = new();

    private List<Event> WhenAvailableQueue { get; } = new();

    public string? Name { get; set; }

    public OperatingTheatre(Simulation environment, string? name = null)
    {
        Environment = environment;
        Environment.Process(OpeningHours());
        Name = name;
    }

    /// <summary>
    ///     Schedule the hospital to reevaluate the request queue every time there is a schedule change in the OTs
    /// </summary>
    private IEnumerable<Event> OpeningHours()
    {
        // PERF: Remove entries as they pass
        foreach (var entry in Allocation.Entries)
        {
            if (entry.EndTime < Environment.StartDate)
                continue;

            if (entry.StartTime < Environment.StartDate)
            {
                _isOpen = true;
                TriggerRequest();
                TriggerWhenAvailable();
                yield return Environment.WaitUntil(entry.EndTime);
                _isOpen = false;
                continue;
            }

            // TODO: Drop wait if start == now?
            yield return Environment.WaitUntil(entry.StartTime);
            _isOpen = true;
            TriggerRequest();
            TriggerWhenAvailable();
            yield return Environment.WaitUntil(entry.EndTime);
            _isOpen = false;
        }
    }

    /// <summary>
    ///     Get the <see cref="Specialty"/> allocated to the theatre at current simulation time.
    /// </summary>
    /// <returns>The allocated <see cref="Specialty"/>, <c>null</c> if there is no current allocation</returns>
    public Specialty? SpecialtyAtTime()
    {
        return SpecialtyAtTime(Environment.Now);
    }

    /// <summary>
    ///     Get the <see cref="Specialty"/> allocated to the theatre at a specified time.
    /// </summary>
    /// <returns>The allocated <see cref="Specialty"/>, <c>null</c> if there is no allocation at that time</returns>
    public Specialty? SpecialtyAtTime(DateTime atTime)
    {
        return Allocation.EntryAt(atTime)?.Value;
    }

    /// <summary>
    ///     Get the remaining time of the current allocation.
    /// </summary>
    /// <returns>The remaining time, <see cref="TimeSpan.Zero"/> if no current allocation</returns>
    public TimeSpan RemainingTime()
    {
        return RemainingTime(Environment.Now);
    }

    /// <summary>
    ///     Get the remaining time of the allocation at the specified time.
    /// </summary>
    /// <returns>The remaining time, <see cref="TimeSpan.Zero"/> if no allocation at that time</returns>
    public TimeSpan RemainingTime(DateTime atTime)
    {
        var entry = Allocation.EntryAt(atTime);
        if (entry is null)
            return TimeSpan.Zero;

        return entry.EndTime - atTime;
    }

    /// <summary>
    ///     Request the <see cref="OperatingTheatre"/> resource.
    /// </summary>
    public Request Request()
    {
        var request = new Request(Environment, TriggerRelease, DisposeCallback);
        RequestQueue.AddLast(request);
        TriggerRequest();
        return request;
    }

    /// <summary>
    ///     Release the <see cref="OperatingTheatre"/> resource.
    /// </summary>
    public Release Release(Request request)
    {
        var release = new Release(Environment, request, TriggerRequest);
        ReleaseQueue.Enqueue(release);
        TriggerRelease();
        return release;
    }

    /// <summary>
    ///     Get an event for when the operating theatre is available again.
    /// </summary>
    /// <param name="changedOnly">Whether to only trigger on change, or immediately on available.</param>
    public Event WhenAvailable(bool changedOnly = true)
    {
        var whenAny = new Event(Environment);
        WhenAvailableQueue.Add(whenAny);

        if (!changedOnly)
            TriggerWhenAvailable();

        return whenAny;
    }

    private void DisposeCallback(Event @event)
    {
        if (@event is Request request)
            Release(request);
    }

    /// <summary>
    ///     Callback to handle the request quque
    /// </summary>
    /// <remarks>
    ///     This is called both when a new request is made, but also when a resource is released as that could free
    ///     up a resource for another request.
    /// </remarks>
    private void TriggerRequest(Event? @event = null)
    {
        while (RequestQueue.First?.Value is
               { } request)
        {
            DoRequest(request);
            if (!request.IsTriggered)
                break;

            RequestQueue.RemoveFirst();
            // TriggerWhenEmpty();
            // TriggerWhenChange();
        }

        // Utilization?.UpdateTo(InUse / (double)Capacity);
        // WIP?.UpdateTo(InUse + RequestQueue.Count);
        // QueueLength?.UpdateTo(RequestQueue.Count);
    }

    /// <summary>
    ///     Callback to handle the release queue
    /// </summary>
    private void TriggerRelease(Event? @event = null)
    {
        while (ReleaseQueue.FirstOrDefault() is
               { } release)
        {
            if (release.Request.IsAlive)
            {
                if (!RequestQueue.Remove(release.Request))
                    throw new InvalidOperationException("Failed to cancel a request.");
                // BreakOffTime?.Add(Environment.ToDouble(Environment.Now - release.Request.Time));
                release.Succeed();
                ReleaseQueue.Dequeue();
                continue;
            }

            DoRelease(release);
            if (!release.IsTriggered)
                break;

            ReleaseQueue.Dequeue();
            TriggerWhenAvailable();
            // TriggerWhenAny();
            // TriggerWhenFull();
            // TriggerWhenChange();
        }

        // Utilization?.UpdateTo(InUse / (double)Capacity);
        // WIP?.UpdateTo(InUse + RequestQueue.Count);
        // QueueLength?.UpdateTo(RequestQueue.Count);
    }

    private void TriggerWhenAvailable()
    {
        if (!IsAvailable || WhenAvailableQueue.Count == 0)
            return;

        foreach (var @event in WhenAvailableQueue)
            @event.Succeed();

        WhenAvailableQueue.Clear();
    }

    /// <summary>
    ///     Carry out the request, i.e. succeed if the <see cref="OperatingTheatre"/> <see cref="IsAvailable"/>
    /// </summary>
    private void DoRequest(Request request)
    {
        if (!IsAvailable)
            return;

        // WaitingTime?.Add(Environment.ToDouble(Environment.Now - request.Time));
        _inUse = true;
        request.Succeed();
    }

    public void DoRelease(Release release)
    {
        // if (!Users.Remove(release.Request))
        //   throw new InvalidOperationException("Released request does not have a user.");
        // LeadTime?.Add(Environment.ToDouble(Environment.Now - release.Request.Time));
        _inUse = false;
        release.Succeed();
    }
}