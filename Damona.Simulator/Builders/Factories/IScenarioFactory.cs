using Damona.Simulator.Resources;
using SimSharp;

namespace Damona.Simulator.Builders.Factories;

/// <summary>
///     Factory for an operating theatre scenario. A scenario in this case is simply a list of available operating
///     theatres and their opening hours.
/// </summary>
public interface IScenarioFactory
{
    IEnumerable<OperatingTheatre> Create(Simulation environment, DateTime fromTime, DateTime toTime);
}