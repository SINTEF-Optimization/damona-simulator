using System.Text.Json.Serialization;

namespace Damona.Schemas.Resources;

public class OperatingTheatreDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("allocations")]
    public required WeeklyAllocationDto Allocation { get; set; }
}