using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Distributions;
using Damona.Simulator.Distributions.Objects;
using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Plans;
using Damona.Simulator.Processes.Emergencies;
using Damona.Simulator.Processes.Operations;
using Damona.Simulator.Processes.Referrals;
using Damona.Simulator.Resources;
using SimSharp;

namespace Damona.Simulator.Builders;

/// <summary>
///     Builds and configures the simulation.
/// </summary>
public class SimulationBuilder : ISimulationBuilder
{
    private readonly DateTime _startTime;
    private readonly DateTime _endTime;
    private SimulationBuilderOptions _options;
    private IRandom _random;

    private IElectiveSurgeryDistribution? _electiveSurgeryDistribution;
    private IEmergencyDistribution? _emergencyDistribution;


    public SimulationBuilder(DateTime startTime, DateTime endTime, SimulationBuilderOptions options)
    {
        _startTime = startTime;
        _endTime = endTime;
        _options = options;
        _random = new PcgRandom(_options.RandomSeed);
        _electiveSurgeryDistribution = _options.ElectiveSurgeryDistributionFactory?.Create(_random);
        _emergencyDistribution = _options.EmergencyDistributionFactory?.Create(_random);
    }

    public SimulationBuilder(DateTime startTime, DateTime endTime, Action<SimulationBuilderOptions> configure)
        : this(startTime, endTime, BuildOptions(configure))
    {
    }


    /// <summary>
    ///     Build a <see cref="HospitalSimulation"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     If wither the <see cref="ElectiveSurgeryDistribution"/>, the <see cref="EmergencyDistribution"/>, or the
    ///     <see cref="IScenarioFactory"/> wasn't initialised earlier in the build process.
    /// </exception>
    public HospitalSimulation Build()
    {
        ArgumentNullException.ThrowIfNull(_electiveSurgeryDistribution);
        ArgumentNullException.ThrowIfNull(_emergencyDistribution);
        ArgumentNullException.ThrowIfNull(_options.ScenarioFactory);

        var env = new HospitalSimulation(_random, _startTime, _endTime);

        // Distributions
        var newReferralsDistribution =
            _options.NewReferralsDistributionFactory?.Create(_random, _electiveSurgeryDistribution) ??
            new NoNewNewReferralsDistribution();

        var newEmergenciesDistribution =
            _options.NewEmergenciesDistributionFactory?.Create(_random, _emergencyDistribution) ??
            new NoNewEmergenciesesDistribution();


        // Plans and waiting lists
        var waitingList = new WaitingList(_options.InitialWaitingList);
        var emergencyList = new EmergencyList(_options.InitialEmergencies);

        // Resources
        var hospital = new Hospital(env, _options.ScenarioFactory.Create(env, _startTime, _endTime));

        // Monitors
        var emergencyMonitor = new SurgeryMonitor<Emergency>(env, emergencyList.GetAll(), collect: true);
        var electiveSurgeryMonitor = new SurgeryMonitor<ElectiveSurgery>(env, waitingList.GetAll(), collect: true);

        env.EmergencyList = emergencyList;
        env.WaitingList = waitingList;
        env.EmergencyMonitor = emergencyMonitor;
        env.ElectiveSurgeryMonitor = electiveSurgeryMonitor;
        env.Hospital = hospital;

        // Processes
        // TODO: Reformat to make them native processes?
        var emergenciesProcess = new NewEmergenciesProcess(env, emergencyList, newEmergenciesDistribution)
        {
            Monitor = emergencyMonitor
        };

        var electiveSurgeryProcess = new NewElectiveSurgeriesProcess(env, waitingList, newReferralsDistribution)
        {
            Monitor = electiveSurgeryMonitor
        };

        var operationsProcess = new ExecuteWaitingListOperationProcess(
            env,
            waitingList,
            emergencyList,
            hospital)
        {
            EmergencyMonitor = emergencyMonitor,
            ElectiveSurgeryMonitor = electiveSurgeryMonitor
        };

        return env;
    }

    /// <summary>
    ///     Adds initial waiting list to the simulator
    /// </summary>
    /// <param name="counts">The numbers of waiting patients of each specialty</param>
    /// <exception cref="ArgumentNullException">
    ///     If the <see cref="IElectiveSurgeryDistribution"/> wasn't initialised in startup.
    /// </exception>
    public void BuildWaitingList(Dictionary<Specialty, int> counts)
    {
        ArgumentNullException.ThrowIfNull(_electiveSurgeryDistribution);

        var result = new List<ElectiveSurgery>();

        foreach (var (specialty, count) in counts)
        {
            for (var i = 0; i < count; i++)
                result.Add(_electiveSurgeryDistribution.Sample(_startTime, specialty));
        }

        _options.InitialWaitingList = result;
    }

    private static SimulationBuilderOptions BuildOptions(Action<SimulationBuilderOptions> configure)
    {
        var options = new SimulationBuilderOptions();
        configure(options);
        return options;
    }
}