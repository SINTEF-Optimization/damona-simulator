using Damona.Schemas.AutoMapper;

namespace Damona.Examples.ScenarioFactories;

/// <summary>
///     Adjusted <see cref="BasePlanScenarioFactory"/> where only 9 operating theatres are operational.
/// </summary>
public class BasePlanWith9TheatresFactory : BasePlanScenarioFactory
{
    public BasePlanWith9TheatresFactory(SpecialtyMap specialtyMap)
        : base(specialtyMap)
    {
        _exclude = [9, 10, 11, 12];
    }
}