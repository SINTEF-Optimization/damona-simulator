using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Domain;

namespace Damona.Simulator.Builders;

/// <summary>
///     Options for the <see cref="SimulationBuilder"/>
/// </summary>
/// <remarks>
///     Some of these options are required, but one cannot accomplish that the way the build process is set up.
/// </remarks>
public class SimulationBuilderOptions
{
    public int RandomSeed { get; set; } = 0;

    public IScenarioFactory? ScenarioFactory { get; set; }

    public IElectiveSurgeryDistributionFactory? ElectiveSurgeryDistributionFactory { get; set; }

    public IEmergencyDistributionFactory? EmergencyDistributionFactory { get; set; }

    public INewEmergenciesDistributionFactory? NewEmergenciesDistributionFactory { get; set; }

    public INewReferralsDistributionFactory? NewReferralsDistributionFactory { get; set; }

    public IEnumerable<ElectiveSurgery> InitialWaitingList { get; set; } = [];

    public IEnumerable<Emergency> InitialEmergencies { get; set; } = [];
}