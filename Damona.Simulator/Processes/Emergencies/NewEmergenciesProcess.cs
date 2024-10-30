using Damona.Simulator.Distributions;
using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Simulator.Processes.Emergencies;

/// <summary>
///     Process that adds new emergency cases to the <see cref="EmergencyList"/>
/// </summary>
public class NewEmergenciesProcess : ActiveObject<Simulation>, IEmergencyProcess
{
    private readonly IEmergencyList _emergencyList;
    private readonly INewEmergenciesDistribution _distribution;

    public ISurgeryMonitor<Emergency>? Monitor { get; set; }

    public NewEmergenciesProcess(
        Simulation environment,
        IEmergencyList emergencyList,
        INewEmergenciesDistribution distribution)
        : base(environment)
    {
        _emergencyList = emergencyList;
        _distribution = distribution;

        Environment.Process(NewEmergencies());
    }

    public IEnumerable<Event> NewEmergencies()
    {
        while (_distribution.Next() is
               { } emergency)
        {
            yield return Environment.WaitUntil(emergency.EmergencyDate);
            _emergencyList.AddEmergency(emergency);
            Monitor?.AddNew(emergency);
        }
    }
}