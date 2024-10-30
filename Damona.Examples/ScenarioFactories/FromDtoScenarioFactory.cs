using Damona.Schemas.AutoMapper;
using Damona.Schemas.Resources;
using Damona.Simulator.Builders.Factories;
using Damona.Simulator.Resources;
using Damona.Simulator.Utilities;
using SimSharp;

namespace Damona.Examples.ScenarioFactories;

/// <summary>
///     A <see cref="IScenarioFactory"/> that creates a scenario from a <see cref="HospitalResourcePlanDto"/>.
/// </summary>
public class FromDtoScenarioFactory(SpecialtyMap map, HospitalResourcePlanDto resourcePlan) : IScenarioFactory
{
    private readonly SpecialtyMap _specialtyMap = map;
    private readonly HospitalResourcePlanDto _resourcePlan = resourcePlan;

    public IEnumerable<OperatingTheatre> Create(Simulation environment, DateTime fromTime, DateTime toTime)
    {
        var operatingTheatres = _resourcePlan.BaseAllocation.OperatingTheatres
            .Select(x => new OperatingTheatre(environment, x.Name))
            .ToList();

        var date = fromTime.MondayCurrentWeek().Date;
        while (date < toTime)
        {
            var index = 0;
            foreach (var otAllocation in _resourcePlan.BaseAllocation.OperatingTheatres)
            {
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Monday, date);
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Tuesday, date.AddDays(1));
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Wednesday, date.AddDays(2));
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Thursday, date.AddDays(3));
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Friday, date.AddDays(4));
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Saturday, date.AddDays(5));
                FillAllocation(operatingTheatres[index], otAllocation.Allocation.Sunday, date.AddDays(6));
                index++;
            }

            date = date.AddDays(7);
        }

        return operatingTheatres;
    }

    private void FillAllocation(OperatingTheatre ot, IEnumerable<AllocationEntryDto> allocations, DateTime date)
    {
        foreach (var allocation in allocations)
            ot.Allocation.NewEntry(
                date + allocation.From,
                allocation.To - allocation.From,
                _specialtyMap.GetOrAdd(allocation.Specialty));
    }
}