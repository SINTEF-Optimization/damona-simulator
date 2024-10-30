using System.Text.Json.Serialization;

namespace Damona.Schemas.Resources;

public class WeeklyHospitalResourcePlanDto
{
    [JsonPropertyName("operatingTheatres")]
    public required IEnumerable<OperatingTheatreDto> OperatingTheatres { get; set; }
}