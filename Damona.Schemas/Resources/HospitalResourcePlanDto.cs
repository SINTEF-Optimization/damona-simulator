using System.Text.Json.Serialization;

namespace Damona.Schemas.Resources;

public class HospitalResourcePlanDto
{
    [JsonPropertyName("base")]
    public required WeeklyHospitalResourcePlanDto BaseAllocation { get; set; }
}