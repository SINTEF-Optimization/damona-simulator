using AutoMapper;
using Damona.Schemas.Common;
using Damona.Schemas.Responses;
using Damona.Simulator.Domain;
using Damona.Simulator.Monitors;
using Damona.Simulator.Monitors.Reports;

namespace Damona.Schemas.AutoMapper;

public class MapProfile : Profile
{
    public MapProfile()
    {
        // WARN: This is unsafe, should use a custom value resolver instead
        CreateMap<int, string>().ConvertUsing<IdToSpecialtyConverter>();
        CreateMap<string, int>().ConvertUsing<SpecialtyToIdConverter>();

        CreateMap<SurgeryReport.PeriodEntry, SimulationPeriodEntry>();
        CreateMap<SurgeryReport.Entry, SimulationSummaryEntry>();

        CreateMap<Emergency, EmergencyDto>();
        CreateMap<ElectiveSurgery, ElectiveSurgeryDto>();
        CreateMap<ExecutedSurgery<Emergency>, ExecutedEmergencyDto>()
            .ForMember(dest => dest.OperatingTheatre, opt => opt.MapFrom(src => src.OperatingTheatre.Name));
        CreateMap<ExecutedSurgery<ElectiveSurgery>, ExecutedElectiveSurgeryDto>()
            .ForMember(dest => dest.OperatingTheatre, opt => opt.MapFrom(src => src.OperatingTheatre.Name));
    }
}