using Damona.Schemas.AutoMapper;
using Damona.Simulator;

namespace Damona.Schemas.Tests.AutoMapper;

public class SpecialtyMapTests
{
    [Fact(DisplayName = "Calling add works as expected")]
    public void GetOrAdd()
    {
        // Arrange:
        var sut = new SpecialtyMap("emergency-0");

        // Act + assert:
        sut.GetOrAdd("specialty-1").Should().Be(0);
        sut.GetOrAdd("specialty-2").Should().Be(1);
        sut.GetOrAdd("specialty-1").Should().Be(0);
        sut.GetOrAdd("specialty-3").Should().Be(2);
        sut.GetOrAdd("specialty-2").Should().Be(1);
        sut.GetOrAdd("emergency-0").Should().Be(Constants.EmergencyId);
    }

    [Fact(DisplayName = "Calling Get retrieves previous IDs")]
    public void Get()
    {
        // Arrange:
        var sut = new SpecialtyMap("emergency-test");
        sut.GetOrAdd("specialty-1");
        sut.GetOrAdd("specialty-2");
        sut.GetOrAdd("specialty-3");

        // Act + assert:
        sut.Get(0).Should().Be("specialty-1");
        sut.Get(1).Should().Be("specialty-2");
        sut.Get(2).Should().Be("specialty-3");
        sut.Get(Constants.EmergencyId).Should().Be("emergency-test");
        sut.Invoking(x => x.Get(3)).Should().Throw<InvalidOperationException>();
    }

    [Fact(DisplayName = "Changing the emergency name works")]
    public void SetEmergencyName()
    {
        // Arrange:
        var sut = new SpecialtyMap("emergency-test");
        sut.GetOrAdd("specialty-1");

        // Act + assert:
        sut.Get(Constants.EmergencyId).Should().Be("emergency-test");
        sut.SetEmergencyName("emergency-new");
        sut.Get(Constants.EmergencyId).Should().Be("emergency-new");
    }
}