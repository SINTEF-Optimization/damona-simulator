using Damona.Simulator.Domain;
using Damona.Simulator.Monitors.Reports;
using Damona.Simulator.Resources;
using SimSharp;

namespace Damona.Simulator.Monitors;

/// <summary>
///     Implementation of <see cref="SurgeryMonitor{TSurgery}"/>.
/// </summary>
public class SurgeryMonitor<TSurgery> : ActiveObject<Simulation>, ISurgeryMonitor<TSurgery> where TSurgery : Surgery
{
    public bool Collect { get; set; }

    public List<TSurgery> New { get; } = new();

    public List<ExecutedSurgery<TSurgery>> Completed { get; } = new();

    public SurgeryReport Report { get; } = new();

    public SurgeryMonitor(Simulation environment, bool collect = false)
        : base(environment)
    {
        Collect = collect;
    }

    public SurgeryMonitor(Simulation environment, IEnumerable<TSurgery> initialSurgeries, bool collect = false)
        : this(environment, collect)
    {
        foreach (var surgery in initialSurgeries)
            Report.AddInitial(surgery.Specialty);
    }

    public void AddNew(TSurgery electiveSurgery)
    {
        Report.AddNew(electiveSurgery.Specialty, Environment.Now);

        if (Collect)
            New.Add(electiveSurgery);
    }

    public void AddCompleted(TSurgery surgery, DateTime startTime, DateTime endTime, OperatingTheatre theatre)
    {
        Report.AddCompleted(surgery.Specialty, Environment.Now);

        if (Collect)
            Completed.Add(
                new()
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    Surgery = surgery,
                    OperatingTheatre = theatre
                });
    }

    SurgeryReport ISurgeryMonitor<TSurgery>.Report()
    {
        return Report;
    }
}