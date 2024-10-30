using AutoMapper;
using Damona.Simulator.Builders;

namespace Damona.Schemas.AutoMapper;

/// <summary>
///     Converts names of specialties to IDs using a <see cref="SpecialtyMap"/>
/// </summary>
public class SpecialtyToIdConverter(SpecialtyMap specialtyMap) : ITypeConverter<string, int>
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;

    public int Convert(string source, int destination, ResolutionContext context)
    {
        var id = _specialtyMap.GetOrAdd(source);
        return id;
    }
}