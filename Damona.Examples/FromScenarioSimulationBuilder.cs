using Damona.Examples.DistributionFactories;
using Damona.Examples.Distributions;
using Damona.Examples.ScenarioFactories;
using Damona.Schemas;
using Damona.Schemas.AutoMapper;
using Damona.Simulator;
using Damona.Simulator.Builders;
using Damona.Simulator.Builders.Factories;

namespace Damona.Examples;

/// <summary>
///     A simulation builder that builds its simulation based on a <see cref="Scenario"/> schema object.
/// </summary>
public class FromScenarioSimulationBuilder(
    SpecialtyMap specialtyMap,
    Scenario scenario) : ISimulationBuilder
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;
    private readonly Scenario _scenario = scenario;

    public HospitalSimulation Build()
    {
        var scenarioFactory = BuildScenarioFactory();
        var electiveSurgeryDistribution = new ElectiveDistributionFromScenarioFactory(_specialtyMap, _scenario);
        var emergencyDistribution = new EmergencyDistributionFromScenarioFactory(_specialtyMap, _scenario);

        var builder = new SimulationBuilder(
            _scenario.StartTime,
            _scenario.EndTime,
            opt =>
            {
                opt.ScenarioFactory = scenarioFactory;
                opt.ElectiveSurgeryDistributionFactory = electiveSurgeryDistribution;
                opt.EmergencyDistributionFactory = emergencyDistribution;

                opt.NewEmergenciesDistributionFactory =
                    new NewEmergenciesNormalUniformDistributionFactory(_scenario.StartTime);

                opt.NewReferralsDistributionFactory =
                    new NewReferralsDistributionFactory(
                        (rng, dist) => new NewReferralsFromOldPlanExecution(rng, dist, _specialtyMap));
            });

        builder.BuildWaitingList(
            _scenario.InitialWaitingList?.ToDictionary(x => _specialtyMap.GetOrAdd(x.Key), x => x.Value) ?? new()
        );

        return builder.Build();
    }

    private IScenarioFactory BuildScenarioFactory()
    {
        if (_scenario.ResourcePlan is null)
            return new BasePlanScenarioFactory(_specialtyMap);

        return new FromDtoScenarioFactory(_specialtyMap, _scenario.ResourcePlan);
    }
}