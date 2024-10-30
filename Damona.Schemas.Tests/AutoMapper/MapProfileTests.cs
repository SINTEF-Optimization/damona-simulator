using AutoMapper;
using Damona.Schemas.AutoMapper;
using Damona.Schemas.Common;
using Damona.Simulator;
using Damona.Simulator.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Damona.Schemas.Tests.AutoMapper;

public class MapProfileTests
{
    private readonly IMapper _mapper;
    private readonly SpecialtyMap _specialtyMap;

    public MapProfileTests()
    {
        var services = new ServiceCollection();
        services.AddAutoMapper(typeof(MapProfile));
        services.AddSingleton(new SpecialtyMap("emergency-test"));

        var provider = services.BuildServiceProvider();
        _mapper = provider.GetService<IMapper>();
        _specialtyMap = provider.GetService<SpecialtyMap>();
    }

    [Fact]
    public void MapIntToString()
    {
        // Arrange:
        _specialtyMap.GetOrAdd("specialty-1");
        _specialtyMap.GetOrAdd("specialty-2");
        _specialtyMap.GetOrAdd("specialty-3");

        // Act + assert:
        _mapper.Map<string>(Constants.EmergencyId).Should().Be("emergency-test");
        _mapper.Map<string>(0).Should().Be("specialty-1");
        _mapper.Map<string>(1).Should().Be("specialty-2");
        _mapper.Map<string>(2).Should().Be("specialty-3");
        _mapper.Invoking(x => x.Map<string>(3)).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MapStringToInt()
    {
        // Arrange:
        _specialtyMap.GetOrAdd("specialty-1");
        _specialtyMap.GetOrAdd("specialty-2");

        // Act + assert:
        _mapper.Map<int>("emergency-test").Should().Be(Constants.EmergencyId);
        _mapper.Map<int>("specialty-1").Should().Be(0);
        _mapper.Map<int>("specialty-2").Should().Be(1);
        _mapper.Map<int>("new-specialty").Should().Be(2);

        _specialtyMap.GetId("new-specialty").Should().Be(2);
    }

    [Fact]
    public void MapElectiveSurgeryToDto()
    {
        // Arrange:
        var specialtyId = _specialtyMap.GetOrAdd("elective-type");
        var surgery = new ElectiveSurgery
        {
            Specialty = specialtyId,
            EstimatedDuration = TimeSpan.FromMinutes(20),
            ReferralDate = DateTime.Parse("2023-04-01T12:30:22")
        };

        // Act + assert:
        _mapper.Map<ElectiveSurgeryDto>(surgery)
            .Should()
            .BeEquivalentTo(
                new ElectiveSurgeryDto
                {
                    Specialty = "elective-type",
                    EstimatedDuration = surgery.EstimatedDuration,
                    ReferralDate = surgery.ReferralDate
                });
    }
}