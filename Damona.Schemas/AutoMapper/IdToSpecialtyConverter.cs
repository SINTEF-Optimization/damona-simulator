using AutoMapper;

namespace Damona.Schemas.AutoMapper;

/// <summary>
///     Converts IDs specialties to names using a <see cref="SpecialtyMap"/>
/// </summary>
public class IdToSpecialtyConverter(SpecialtyMap specialtyMap) : ITypeConverter<int, string>
{
    private readonly SpecialtyMap _specialtyMap = specialtyMap;

    public string Convert(int source, string destination, ResolutionContext context)
    {
        return _specialtyMap.Get(source);
    }
}