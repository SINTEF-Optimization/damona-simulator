using System.Text.Json.Serialization;
using Damona.Schemas.Common;

namespace Damona.Schemas.Resources;

public class AllocationEntryDto
{
    [JsonPropertyName("specialty")]
    public required string Specialty { get; set; }

    [JsonPropertyName("from")]
    public required TimeSpan From { get; set; }

    [JsonPropertyName("to")]
    public required TimeSpan To { get; set; }
}