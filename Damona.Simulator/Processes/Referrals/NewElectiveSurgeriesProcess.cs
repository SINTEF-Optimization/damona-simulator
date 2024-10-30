using Damona.Simulator.Distributions;
using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Simulator.Processes.Referrals;

/// <summary>
///     Process that adds new elective surgeries to the <see cref="IWaitingList"/>
/// </summary>
public class NewElectiveSurgeriesProcess : ActiveObject<Simulation>, IElectiveSurgeryProcess
{
    private readonly IWaitingList _waitingList;
    private readonly INewReferralsDistribution _distribution;

    public ISurgeryMonitor<ElectiveSurgery>? Monitor { get; set; }

    public NewElectiveSurgeriesProcess(
        Simulation env,
        IWaitingList waitingList,
        INewReferralsDistribution distribution)
        : base(env)
    {
        _distribution = distribution;
        _waitingList = waitingList;
        Environment.Process(MonthlyElectiveSurgeries());
    }

    public IEnumerable<Event> MonthlyElectiveSurgeries()
    {
        if (Environment.Now.Day != 1)
            throw new ArgumentException("Expected the simulation to start at the first day of the month");

        while (true)
        {
            var surgeries = _distribution.NewSurgeriesInMonth(Environment.Now);
            _waitingList.AddElectiveSurgeries(surgeries);

            foreach (var surgery in surgeries)
                Monitor?.AddNew(surgery);

            yield return Environment.WaitUntil(Environment.Now.StartOfNextMonth());
        }
    }
}