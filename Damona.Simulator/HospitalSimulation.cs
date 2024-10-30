using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Resources;
using SimSharp;

namespace Damona.Simulator;

/// <summary>
///     A <see cref="Simulation"/> of a <see cref="Hospital"/> and the surgeries that are executed at it.
/// </summary>
public class HospitalSimulation : Simulation
{
    public DateTime EndTime { get; set; }

    public IEmergencyList? EmergencyList { get; set; }
    public IWaitingList? WaitingList { get; set; }

    public ISurgeryMonitor<Emergency>? EmergencyMonitor { get; set; }
    public ISurgeryMonitor<ElectiveSurgery>? ElectiveSurgeryMonitor { get; set; }

    public Hospital? Hospital { get; set; }

    public HospitalSimulation(IRandom random, DateTime startTime, DateTime endTime)
        : base(random, startTime)
    {
        EndTime = endTime;
    }

    public override object Run(Event? stopEvent = null)
    {
        Validate();
        
        if (stopEvent is not null)
            return base.Run(stopEvent);

        return base.Run(EndTime);
    }

    public override object Run(TimeSpan span)
    {
        Validate();
        
        if (span > EndTime - StartDate)
            span = EndTime - StartDate;

        return base.Run(span);
    }

    public override object Run(DateTime until)
    {
        Validate();
        
        if (until > EndTime)
            until = EndTime;

        return base.Run(until);
    }

    /// <summary>
    ///     Validate that the necessary objects have been set.
    /// </summary>
    /// <remarks>
    ///     Due to circular dependencies between the objects, we cannot guarantee that the <see cref="EmergencyList"/>
    ///     and <see cref="WaitingList"/> aren't <code>null</code>, but they must have been set before the code runs.
    /// </remarks>
    private void Validate()
    {
        ArgumentNullException.ThrowIfNull(EmergencyList);
        ArgumentNullException.ThrowIfNull(WaitingList);
    }
}