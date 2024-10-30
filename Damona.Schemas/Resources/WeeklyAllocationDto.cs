using System.Text.Json.Serialization;

namespace Damona.Schemas.Resources;

public class WeeklyAllocationDto
{
    [JsonPropertyName("monday")]
    public required IEnumerable<AllocationEntryDto> Monday { get; set; }

    [JsonPropertyName("tuesday")]
    public required IEnumerable<AllocationEntryDto> Tuesday { get; set; }

    [JsonPropertyName("wednesday")]
    public required IEnumerable<AllocationEntryDto> Wednesday { get; set; }

    [JsonPropertyName("thursday")]
    public required IEnumerable<AllocationEntryDto> Thursday { get; set; }

    [JsonPropertyName("friday")]
    public required IEnumerable<AllocationEntryDto> Friday { get; set; }

    [JsonPropertyName("saturday")]
    public required IEnumerable<AllocationEntryDto> Saturday { get; set; }

    [JsonPropertyName("sunday")]
    public required IEnumerable<AllocationEntryDto> Sunday { get; set; }
}